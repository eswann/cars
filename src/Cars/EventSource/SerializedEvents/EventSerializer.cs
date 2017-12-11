using System;
using System.Collections.Generic;
using Cars.Core;
using Cars.Events;

namespace Cars.EventSource.SerializedEvents
{
    public class EventSerializer : IEventSerializer
    {
        private readonly ITextSerializer _textSerializer;

        public EventSerializer(ITextSerializer textSerializer)
        {
            _textSerializer = textSerializer;
        }

        public ISerializedEvent Serialize(IDomainEvent @event, IEnumerable<KeyValuePair<string, object>> metadatas)
        {
            var metadata = new Metadata(metadatas);
            
            var aggregateId = metadata.GetValue(MetadataKeys.AggregateId, (value) => Guid.Parse(value.ToString()));
            var version = metadata.GetValue(MetadataKeys.StreamSequenceNumber, (value) => int.Parse(value.ToString()));

            var serializedData = _textSerializer.Serialize(@event);
            var serializedMetadata = _textSerializer.Serialize(metadata);

            return new SerializedEvent(aggregateId, version, serializedData, serializedMetadata, metadata);
        }

        public IDomainEvent Deserialize(ICommitedEvent commitedEvent)
        {
            var metadata = _textSerializer.Deserialize<Metadata>(commitedEvent.SerializedMetadata);

            var eventClrType = metadata.GetValue(MetadataKeys.EventClrType, (value) => value.ToString());
            
            return (IDomainEvent) _textSerializer.Deserialize(commitedEvent.SerializedData, eventClrType);
        }
    }
}