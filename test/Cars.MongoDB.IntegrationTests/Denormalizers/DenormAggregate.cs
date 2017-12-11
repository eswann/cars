using System;
using Cars.EventSource;

namespace Cars.MongoDB.IntegrationTests.Denormalizers
{
    public class DenormAggregate : Aggregate
    {
        public DenormAggregate()
        {
        }

        public DenormAggregate(Guid aggregateId)
        {
            Emit(new ThingCreated(aggregateId));
        }

        public string LastThing { get; private set; }


        public void DoThing(string text)
        {
            Emit(new ThingHappened(text));
        }

        protected override void RegisterEvents()
        {
            SubscribeTo<ThingCreated>(e => { });

            SubscribeTo<ThingHappened>(e =>
            {
                AggregateId = e.AggregateId;
                LastThing = e.Text;
            });

        }
    }
}