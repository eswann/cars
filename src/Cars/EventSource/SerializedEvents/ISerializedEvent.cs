using System;

namespace Cars.EventSource.SerializedEvents
{
    public interface ISerializedEvent
    {
        Guid AggregateId { get; }
        int Version { get; }
        string SerializedMetadata { get; }
        string SerializedData { get; }
        IMetadata Metadata { get; }
    }
}