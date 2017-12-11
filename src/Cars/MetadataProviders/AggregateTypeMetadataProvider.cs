using System.Collections.Generic;
using Cars.Events;
using Cars.EventSource;

namespace Cars.MetadataProviders
{
    public class StreamTypeMetadataProvider : IMetadataProvider
    {
        public IEnumerable<KeyValuePair<string, object>> Provide<TAggregate>(TAggregate aggregate, IDomainEvent @event, IMetadata metadata) where TAggregate : IAggregate
        {
            yield return new KeyValuePair<string, object>(MetadataKeys.AggregateId, aggregate.AggregateId);
            yield return new KeyValuePair<string, object>(MetadataKeys.StreamSequenceNumber, aggregate.UncommittedVersion);
            yield return new KeyValuePair<string, object>(MetadataKeys.StreamTypeFullname, aggregate.GetType().FullName);
        }
    }
}