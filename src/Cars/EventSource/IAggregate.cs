using System;
using System.Collections.Generic;
using Cars.Collections;
using Cars.Events;

namespace Cars.EventSource
{
    public interface IAggregate
    {
        Guid AggregateId { get; }
        int Version { get; }

        int UncommittedVersion { get; }
        IReadOnlyCollection<IUncommitedEvent> UncommitedEvents { get; }
        void ClearUncommitedEvents();

        void LoadFromHistory(CommitedDomainEventCollection events);
    }
}