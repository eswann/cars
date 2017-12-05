// The MIT License (MIT)
// 
// Copyright (c) 2016 Nelson Corrêa V. Júnior
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cars.Collections;
using Cars.Core;
using Cars.Events;
using Cars.EventSource.Exceptions;
using Cars.EventSource.Projections;
using Cars.EventSource.Snapshots;
using Cars.Extensions;
using Cars.MessageBus;
using Cars.MetadataProviders;
using Microsoft.Extensions.Logging;

namespace Cars.EventSource.Storage
{
    public class Session : ISession
    {
        private readonly StreamTracker _streamTracker = new StreamTracker();
        private readonly List<Aggregate> _aggregates = new List<Aggregate>();
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEventSerializer _eventSerializer;
        private readonly ISnapshotSerializer _snapshotSerializer;
        private readonly IProjectionSerializer _projectionSerializer;
        private readonly IProjectionProviderScanner _projectionProviderScanner;
        private readonly IEventUpdateManager _eventUpdateManager;
        private readonly ISnapshotStrategy _snapshotStrategy;
        private readonly IEnumerable<IMetadataProvider> _metadataProviders;
        private readonly ILogger _logger;

        private bool _externalTransaction;

        public IReadOnlyList<IAggregate> Aggregates => _aggregates;

        public Session(
            ILoggerFactory loggerFactory,
            IEventStore eventStore,
            IEventPublisher eventPublisher,
            IEventSerializer eventSerializer,
            ISnapshotSerializer snapshotSerializer,
            IProjectionSerializer projectionSerializer,
            IProjectionProviderScanner projectionProviderScanner = null,
            IEventUpdateManager eventUpdateManager = null,
            IEnumerable<IMetadataProvider> metadataProviders = null,
            ISnapshotStrategy snapshotStrategy = null)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            if (metadataProviders == null) metadataProviders = Enumerable.Empty<IMetadataProvider>();

            _logger = loggerFactory.CreateLogger<Session>();

            _snapshotStrategy = snapshotStrategy ?? new IntervalSnapshotStrategy();
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _snapshotSerializer = snapshotSerializer ?? throw new ArgumentNullException(nameof(snapshotSerializer));
            _projectionSerializer = projectionSerializer ?? throw new ArgumentNullException(nameof(projectionSerializer));
            _eventUpdateManager = eventUpdateManager;
            _metadataProviders = metadataProviders.Concat(new IMetadataProvider[]
            {
                new StreamTypeMetadataProvider(),
                new EventTypeMetadataProvider(),
                new CorrelationIdMetadataProvider()
            });
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
			_projectionProviderScanner = projectionProviderScanner ?? new ProjectionProviderAttributeScanner();

        }

        /// <summary>
        /// Retrieves an stream, load your historical events and add to tracking.
        /// </summary>
        /// <typeparam name="TProjection"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="StreamNotFoundException"></exception>
        public async Task<TProjection> GetByIdAsync<TProjection>(Guid id) where TProjection : AggregateProjection, new()
        {
            _logger.LogDebug($"Getting stream '{typeof(TProjection).FullName}' with identifier: '{id}'.");           
            var isMutator = typeof(TProjection).GetTypeInfo().IsSubclassOf(typeof(Aggregate));

            _logger.LogDebug("Returning an stream tracked.");

            TProjection projection;
            if (isMutator)
            {
                projection =_streamTracker.GetById<TProjection>(id);
                if (projection != null)
                {
                    RegisterForTracking(projection as Aggregate);
                    return projection;
                }
            }

            projection = new TProjection();

            IEnumerable<ICommitedEvent> events;

            _logger.LogDebug("Checking if stream has snapshot support.");

            if (_snapshotStrategy.CheckSnapshotSupport(projection.GetType()))
            {
                if (projection is ISnapshotAggregate snapshotStream)
                {
                    int version = 0;
                    var snapshot = await _eventStore.GetLatestSnapshotByIdAsync(id).ConfigureAwait(false);

                    if (snapshot != null)
                    {
                        version = snapshot.Version;

                        _logger.LogDebug("Restoring snapshot.");

                        var snapshotRestore = _snapshotSerializer.Deserialize(snapshot);

                        snapshotStream.Restore(snapshotRestore);

                        _logger.LogDebug("Snapshot restored.");
                    }

                    events = await _eventStore.GetEventsForwardAsync(id, version).ConfigureAwait(false);

                    LoadStream(projection, events);
                }
            }
            else
            {
                events = await _eventStore.GetAllEventsAsync(id).ConfigureAwait(false);

                LoadStream(projection, events);
            }

            if (projection.AggregateId.Equals(Guid.Empty))
            {
                _logger.LogError($"The stream ({typeof(TProjection).FullName} {id}) was not found.");

                throw new StreamNotFoundException(typeof(TProjection).Name, id);
            }

            if (isMutator)
            {
                RegisterForTracking(projection as Aggregate);
            }

            return projection;
        }

        /// <summary>
        /// Add the stream to tracking.
        /// </summary>
        /// <typeparam name="TAggregate"></typeparam>
        /// <param name="aggregate"></param>
        public Task AddAsync<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            CheckConcurrency(aggregate);

            RegisterForTracking(aggregate);
            
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Start transaction.
        /// </summary>
        public void BeginTransaction()
        {
            _logger.LogInformation($"Called method: {nameof(Session)}.{nameof(BeginTransaction)}.");

            if (_externalTransaction)
            {
                _logger.LogError("Transaction already was open.");

                throw new InvalidOperationException("The transaction already was open.");
            }

            _externalTransaction = true;
            _eventStore.BeginTransaction();
        }

        /// <summary>
        /// Confirm changes.
        /// </summary>
        /// <returns></returns>
        public async Task CommitAsync()
        {
            _logger.LogInformation($"Called method: {nameof(Session)}.{nameof(CommitAsync)}.");

            _logger.LogInformation($"Calling method: {_eventStore.GetType().Name}.{nameof(CommitAsync)}.");

            await _eventStore.CommitAsync().ConfigureAwait(false);
            _externalTransaction = false;
        }

        /// <summary>
        /// Call <see cref="IEventStore.SaveAsync"/> in <see cref="IEventStore"/> passing serialized events.
        /// </summary>
        public virtual async Task SaveChangesAsync()
        {
            _logger.LogInformation($"Called method: {nameof(Session)}.{nameof(SaveChangesAsync)}.");

            if (!_externalTransaction)
            {
                _eventStore.BeginTransaction();
            }

            // If transaction called externally, the client should care with transaction.

            try
            {
                _logger.LogInformation("Serializing events.");

                var uncommitedEvents =
                    _aggregates.SelectMany(e => e.UncommitedEvents)
                    .OrderBy(o => o.CreatedAt)
                    .Cast<UncommitedEvent>()
                    .ToList();
                
                var serializedEvents = uncommitedEvents.Select(uncommitedEvent =>
                {
                    var metadatas = _metadataProviders.SelectMany(md => md.Provide(uncommitedEvent.Aggregate,
                        uncommitedEvent.OriginalEvent,
                        Metadata.Empty)).Concat(new[]
                    {
                        new KeyValuePair<string, object>(MetadataKeys.EventId, Guid.NewGuid()),
                        new KeyValuePair<string, object>(MetadataKeys.EventVersion, uncommitedEvent.Version)
                    });

                    var serializeEvent = _eventSerializer.Serialize(uncommitedEvent.Aggregate,
                        uncommitedEvent.OriginalEvent,
                        metadatas);

                    return serializeEvent;
                });

                _logger.LogInformation("Saving events on Event Store.");

                await _eventStore.SaveAsync(serializedEvents).ConfigureAwait(false);

                _logger.LogInformation("Begin iterate in collection of stream.");

                foreach (var stream in _aggregates)
                {
                    _logger.LogInformation($"Checking if should take snapshot for stream: '{stream.AggregateId}'.");

                    if (_snapshotStrategy.ShouldMakeSnapshot(stream))
                    {
                        _logger.LogInformation("Taking stream's snapshot.");

                        await stream.TakeSnapshot(_eventStore, _snapshotSerializer).ConfigureAwait(false);
                    }

                    _logger.LogInformation($"Update stream's version from {stream.Version} to {stream.UncommittedVersion}.");

                    stream.SetVersion(stream.UncommittedVersion);

                    stream.ClearUncommitedEvents();

                    _logger.LogInformation($"Scanning projection providers for {stream.GetType().Name}.");

                    var scanResult = await _projectionProviderScanner.ScanAsync(stream.GetType()).ConfigureAwait(false);

                    _logger.LogInformation($"Projection providers found: {scanResult.Providers.Count()}.");

                    foreach (var provider in scanResult.Providers)
                    {
                        var projection = provider.CreateProjection(stream);

                        var projectionSerialized = _projectionSerializer.Serialize(stream.AggregateId, projection);

                        await _eventStore.SaveProjectionAsync(projectionSerialized).ConfigureAwait(false);
                    }
                                        
                }

                _logger.LogDebug("End iterate.");

                _logger.LogInformation($"Publishing events. [Qty: {uncommitedEvents.Count}]");

                await _eventPublisher.PublishAsync(uncommitedEvents.Select(e => e.OriginalEvent)).ConfigureAwait(false);

                _logger.LogInformation("Published events.");
                
                _aggregates.Clear();

                await _eventPublisher.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);

                if (!_externalTransaction)
                {
                    Rollback();
                }

                throw;
            }

            if (!_externalTransaction)
            {
                await _eventStore.CommitAsync().ConfigureAwait(false);
            }
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

            _logger.LogDebug("Cleaning tracker.");

            foreach (var stream in _aggregates)
            {
                _streamTracker.Remove(stream.GetType(), stream.AggregateId);
            }

            _aggregates.Clear();
        }

        private void RegisterForTracking<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            _logger.LogDebug($"Adding to track: {aggregate.GetType().FullName}.");

            if (_aggregates.All(e => e.AggregateId != aggregate.AggregateId))
            {
                _aggregates.Add(aggregate);
            }

            _streamTracker.Add(aggregate);
        }

        private void CheckConcurrency<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate
        {
            _logger.LogDebug("Checking concurrency.");

            var trackedStream = _streamTracker.GetById<TAggregate>(aggregate.AggregateId);

            if (trackedStream == null) return;

            if (trackedStream.Version != aggregate.Version)
            {
                _logger.LogError($"Aggregate's current version is: {aggregate.Version} - expected is: {trackedStream.Version}.");

                throw new ExpectedVersionException<TAggregate>(aggregate, trackedStream.Version);
            }
        }

        private void LoadStream<TProjection>(TProjection projection, IEnumerable<ICommitedEvent> commitedEvents) where TProjection : AggregateProjection
        {
            var flatten = commitedEvents as ICommitedEvent[] ?? commitedEvents.ToArray();

            if (flatten.Any())
            {
                var events = flatten.Select(_eventSerializer.Deserialize);

                if (_eventUpdateManager != null)
                {
                    _logger.LogDebug("Calling Update Manager");

                    events = _eventUpdateManager.Update(events);
                }

                projection.LoadFromHistory(new CommitedDomainEventCollection(events));
                projection.SetVersion(flatten.Select(e => e.Version).Max());
            }
        }
    }
}