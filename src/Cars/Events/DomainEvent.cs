using System;

namespace Cars.Events
{
    /// <summary>
    /// Used to represent an Domain event.
    /// The domain event are things that have value for your domain.
    /// They are raised when occur changes on the stream root.
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        /// <summary>
        /// Aggregate Unique identifier.
        /// </summary>
        public Guid AggregateId { get; protected set; }

        public virtual DomainEventMetadata Metadata { get; internal set; }
        
        /// <summary>
        /// Empty constructor is needed for serialization
        /// </summary>
        protected DomainEvent() { }

        /// <summary>
        /// Construct the domain event.
        /// </summary>
        /// <param name="aggregateId"></param>
        protected DomainEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}