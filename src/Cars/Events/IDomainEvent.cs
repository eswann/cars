using System;

namespace Cars.Events
{
    /// <summary>
    /// Used to represent an Domain event.
    /// The domain event are things that have value for your domain.
    /// They are raised when occur changes on the stream root.
    /// </summary>
    public interface IDomainEvent
    {
        Guid AggregateId { get; }

        DomainEventMetadata Metadata { get; }
    }
}