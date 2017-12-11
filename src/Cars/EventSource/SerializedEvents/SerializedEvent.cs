using System;

namespace Cars.EventSource.SerializedEvents
{
    public class SerializedEvent : ISerializedEvent
    {
        public SerializedEvent(Guid aggregateId,
            int version,
            string serializedData,
            string serializedMetadata,
            IMetadata metadata)
        {
            AggregateId = aggregateId;
            Version = version;
            SerializedData = serializedData;
            SerializedMetadata = serializedMetadata;
            Metadata = metadata;
        }

        public Guid AggregateId { get; }
        public int Version { get; }
        public string SerializedMetadata { get; }
        public string SerializedData { get; }
        public IMetadata Metadata { get; }
    }
}