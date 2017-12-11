using System;

namespace Cars.Events
{
    public interface ICommitedEvent
    {
        Guid AggregateId { get; }
        int Version { get; }
        string SerializedData { get; }
        string SerializedMetadata { get; }
    }
}