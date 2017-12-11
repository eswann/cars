using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cars.Collections;
using Cars.Events;

namespace Cars.EventSource
{
    public abstract class Aggregate : IAggregate
    {
        private readonly Route<IDomainEvent> _routeEvents = new Route<IDomainEvent>();
        private readonly List<UncommitedEvent> _uncommitedEvents = new List<UncommitedEvent>();

        /// <summary>
        /// Aggregate default constructor.
        /// </summary>
        protected Aggregate()
        {
            RegisterEvents();
        }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid AggregateId { get; protected set; }

        /// <summary>
        /// Current version of the Aggregate.
        /// </summary>
        public int Version { get; protected set; }


        /// <summary>
        /// Collection of <see cref="IDomainEvent"/> that contains uncommited events.
        /// All events that not persisted yet should be here.
        /// </summary>
        public IReadOnlyCollection<IUncommitedEvent> UncommitedEvents => _uncommitedEvents;

        /// <summary>
        /// This version is calculated based on Version + Uncommited events count.
        /// </summary>
        public int UncommittedVersion => Version + _uncommitedEvents.Count;

        /// <summary>
        /// Event emitter.
        /// </summary>
        /// <param name="event"></param>
        protected void Emit(DomainEvent @event)
        {
            Task.WaitAll(Task.Delay(1));
            @event.Metadata = new DomainEventMetadata(DateTime.UtcNow, UncommittedVersion + 1);
            _uncommitedEvents.Add(new UncommitedEvent(this, @event, @event.Metadata.Sequence));

            ApplyEvent(@event);
        }

        /// <summary>
        /// This method is called internaly and you can put all handlers here.
        /// </summary>
        protected abstract void RegisterEvents();

        protected void SubscribeTo<T>(Action<T> action)
            where T : class, IDomainEvent
        {
            _routeEvents.Add(typeof(T), o => action(o as T));
        }


        /// <summary>
        /// Apply the event in Aggregate and store the event in Uncommited list.
        /// The last event applied is the current state of the Aggregate.
        /// </summary>
        /// <param name="event"></param>
        private void ApplyEvent(IDomainEvent @event)
        {
            _routeEvents.Handle(@event);
        }
    
        /// <summary>
        /// Clear the collection of events that uncommited.
        /// </summary>
        public void ClearUncommitedEvents()
        {
            _uncommitedEvents.Clear();
        }

        /// <summary>
        /// Load the events in the Aggregate.
        /// </summary>
        public void LoadFromHistory(CommitedDomainEventCollection events)
        {
            foreach (var @event in events)
            {
                ApplyEvent(@event);
            }
        }

        /// <summary>
        /// Update stream's version.
        /// </summary>
        /// <param name="version"></param>
        internal void SetVersion(int version)
        {
            Version = version;
        }

    }
}