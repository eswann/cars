using System.Collections.Generic;
using Cars.Events;
using Cars.EventSource;
using Cars.Extensions;

namespace Cars.MetadataProviders
{
    public class EventTypeMetadataProvider : IMetadataProvider
    {
        public IEnumerable<KeyValuePair<string, object>> Provide<TAggregate>(TAggregate aggregate, IDomainEvent @event, IMetadata metadata)
            where TAggregate : IAggregate
        {
            if (!@event.TryGetEventNameAttribute(out var eventName))
            {
                eventName = @event.GetType().FullName;
            }

            yield return new KeyValuePair<string, object>(MetadataKeys.EventClrType, @event.GetType().AssemblyQualifiedName);
            yield return new KeyValuePair<string, object>(MetadataKeys.EventName, eventName);
        }
    }
}