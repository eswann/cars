using System;
using Cars.Events;
using MongoDB.Bson;

namespace Cars.EventStore.MongoDB
{
    internal class MongoCommitedEvent : ICommitedEvent
    {
        public Guid AggregateId { get; private set; }
        public int Version { get; private set; }
        public string SerializedData { get; private set; }
        public string SerializedMetadata { get; private set; }

        public static MongoCommitedEvent Create(Event e)
        {
            return new MongoCommitedEvent
            {
                AggregateId = e.AggregateId,
                Version = e.Version,
                SerializedData = e.EventData.ToJson(),
                SerializedMetadata = e.Metadata.ToJson(),
            };
        }
    }
}