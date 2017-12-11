using System.Collections.Generic;
using Cars.Events;

namespace Cars.EventSource.SerializedEvents
{
    public interface IEventSerializer
    {
        ISerializedEvent Serialize(IDomainEvent @event, IEnumerable<KeyValuePair<string, object>> metadatas);
        IDomainEvent Deserialize(ICommitedEvent commitedEvent);
    }
}