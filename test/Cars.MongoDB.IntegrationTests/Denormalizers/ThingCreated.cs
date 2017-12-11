using System;
using Cars.Events;

namespace Cars.MongoDB.IntegrationTests.Denormalizers
{
    public class ThingCreated : DomainEvent
    {
        public ThingCreated()
        {
        }

        public ThingCreated(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

    }
}
