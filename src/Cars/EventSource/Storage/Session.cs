using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cars.Collections;
using Cars.Core;
using Cars.Events;
using Cars.EventSource.Exceptions;
using Cars.EventSource.SerializedEvents;
using Cars.MessageBus;
using Cars.MetadataProviders;
using Microsoft.Extensions.Logging;

namespace Cars.EventSource.Storage
{
    public class Session : ISession
    {
        private readonly AggregateTracker _aggregateTracker = new AggregateTracker();
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEventSerializer _eventSerializer;
        private readonly IEventUpdateManager _eventUpdateManager;
        private readonly IEnumerable<IMetadataProvider> _metadataProviders;
        private readonly ILogger _logger;

        private bool _transactionStarted;

        public IEnumerable<IAggregate> Aggregates => _aggregateTracker.Aggregates;

        public Session(
            ILoggerFactory loggerFactory,
            IEventStore eventStore,
            IEventPublisher eventPublisher,
            IEventSerializer eventSerializer,
            IEventUpdateManager eventUpdateManager = null,
            IEnumerable<IMetadataProvider> metadataProviders = null)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            if (metadataProviders == null) metadataProviders = Enumerable.Empty<IMetadataProvider>();

            _logger = loggerFactory.CreateLogger<Session>();

            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _eventUpdateManager = eventUpdateManager;
            _metadataProviders = metadataProviders.Concat(new IMetadataProvider[]
            {
                new StreamTypeMetadataProvider(),
                new EventTypeMetadataProvider(),
                new CorrelationIdMetadataProvider()
            });
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        /// <summary>
        /// Retrieves an stream, load your historical events and add to tracking.
        /// </summary>
        /// <exception cref="AggregateNotFoundException"></exception>
        public async Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : Aggregate, new()
        {
            _logger.LogDebug($"Getting stream '{typeof(TAggregate).FullName}' with identifier: '{id}'.");
            _logger.LogDebug("Returning an aggregate tracked.");

            TAggregate aggregate = _aggregateTracker.GetById<TAggregate>(id);
            if (aggregate != null)
            {
                return aggregate;
            }

            aggregate = new TAggregate();

            var events = await _eventStore.GetEventsByAggregateId(id).ConfigureAwait(false);
            LoadStream(aggregate, events);
            
            if (aggregate.AggregateId.Equals(Guid.Empty))
            {
                _logger.LogError($"The stream ({typeof(TAggregate).FullName} {id}) was not found.");
                throw new AggregateNotFoundException(typeof(TAggregate).Name, id);
            }

            RegisterForTracking(aggregate);
            return aggregate;
        }

        /// <summary>
        /// Add the stream to tracking.
        /// </summary>
        public void Add<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            if (aggregate.Version == 0)
            {
                RegisterForTracking(aggregate);
            }
        }

        /// <summary>
        /// Start transaction.
        /// </summary>
        public void BeginTransaction()
        {
            _logger.LogInformation($"Called method: {nameof(Session)}.{nameof(BeginTransaction)}.");

            if (_transactionStarted)
            {
                _logger.LogError("Transaction already was open.");

                throw new InvalidOperationException("The transaction already was open.");
            }
            _transactionStarted = true;
            _eventStore.BeginTransaction();
        }


        /// <summary>
        /// Rollback <see cref="IEventPublisher"/>, <see cref="IEventStore"/> and remove stream tracking.
        /// </summary>
        public void Rollback()
        {
            _logger.LogDebug("Calling Event Publisher Rollback.");

            _eventPublisher.Rollback();

            _logger.LogDebug("Calling Event Store Rollback.");

            _eventStore.Rollback();
            _aggregateTracker.Clear();
            _transactionStarted = false;
        }

        /// <summary>
        /// Call <see cref="IEventStore.SaveAsync"/> in <see cref="IEventStore"/> passing serialized events.
        /// </summary>
        public virtual async Task CommitAsync()
        {
            _logger.LogInformation($"Called method: {nameof(Session)}.{nameof(CommitAsync)}.");

            var aggregatesWithCommits = Aggregates.Where(x => x.UncommitedEvents.Count > 0).ToList();
            AssertNoDuplicateCommits(aggregatesWithCommits);

            if (!_transactionStarted)
            {
                _eventStore.BeginTransaction();
            }

            try
            {
                _logger.LogDebug("Serializing events.");
                var uncommitedEvents =
                    aggregatesWithCommits.SelectMany(e => e.UncommitedEvents)
                    .OrderBy(o => o.CreatedAt)
                    .Cast<UncommitedEvent>()
                    .ToList();

                var serializedEvents = uncommitedEvents.Select(uncommitedEvent =>
                {
                    var metadata = _metadataProviders.SelectMany(md => md.Provide(uncommitedEvent.Aggregate,
                        uncommitedEvent.OriginalEvent,
                        Metadata.Empty)).Concat(new[]
                    {
                        new KeyValuePair<string, object>(MetadataKeys.EventId, Guid.NewGuid()),
                        new KeyValuePair<string, object>(MetadataKeys.EventVersion, uncommitedEvent.Version)
                    });

                    var serializedEvent = _eventSerializer.Serialize(uncommitedEvent.OriginalEvent, metadata);
                    return serializedEvent;
                }).ToList();

                _logger.LogDebug("Saving events on Event Store.");
                await _eventStore.SaveAsync(serializedEvents).ConfigureAwait(false);

                _logger.LogDebug("Begin iterate in collection of stream.");
                foreach (var aggregate in _aggregateTracker.Aggregates)
                {
                    _logger.LogDebug($"Update stream's version from {aggregate.Version} to {aggregate.UncommittedVersion}.");
                    aggregate.SetVersion(aggregate.UncommittedVersion);
                    aggregate.ClearUncommitedEvents();
                    _logger.LogDebug($"Scanning projection providers for {aggregate.GetType().Name}.");
                }
                _logger.LogInformation($"Publishing events. [Qty: {uncommitedEvents.Count}]");

                await _eventPublisher.EnqueueAsync(uncommitedEvents.Select(e => e.OriginalEvent)).ConfigureAwait(false);
                _logger.LogDebug("Published events.");

                _aggregateTracker.Clear();

                _logger.LogDebug($"Calling method: {_eventStore.GetType().Name}.{nameof(CommitAsync)}.");
                await _eventStore.CommitAsync().ConfigureAwait(false);
                _transactionStarted = false;

                await _eventPublisher.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                Rollback();
                throw;
            }
        }

        private static void AssertNoDuplicateCommits(List<IAggregate> aggregatesWithCommits)
        {
            var groupeDups = aggregatesWithCommits.GroupBy(c => c.AggregateId).Where(g => g.Skip(1).Any()).ToList();
            if (groupeDups.Count > 0)
            {
                throw new AggregateConcurrencyException(groupeDups, "Saving multiple representations of the same aggregate is not permitted.");
            }
        }

        private void RegisterForTracking<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            _logger.LogDebug($"Adding to track: {aggregate.GetType().FullName}.");
            _aggregateTracker.Add(aggregate);
        }

        private void LoadStream(Aggregate aggregate, IList<ICommitedEvent> commitedEvents)
        {

            if (commitedEvents.Count > 0)
            {
                var events = commitedEvents.Select(_eventSerializer.Deserialize);

                if (_eventUpdateManager != null)
                {
                    _logger.LogDebug("Calling Update Manager");

                    events = _eventUpdateManager.Update(events);
                }

                aggregate.LoadFromHistory(new CommitedDomainEventCollection(events));
                aggregate.SetVersion(commitedEvents.Select(e => e.Version).Max());
            }
        }

    }
}