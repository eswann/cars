using System;

namespace Cars.Events
{
    public interface IUncommitedEvent
    {
        DateTime CreatedAt { get; }
        int Version { get; }
        IDomainEvent OriginalEvent { get; }
    }
}