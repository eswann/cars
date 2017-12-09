using System;
using Cars.EventSource;
using Cars.UnitTests.Domain.Stubs.Events;

namespace Cars.UnitTests.Domain.Stubs
{
    public class StubSubAggregate : Aggregate
    {
        public string Name { get; private set; }
        public Guid RelatedId { get; private set; }

        private StubSubAggregate(Guid newGuid, string name)
        {
            Emit(new StubAggregateCreatedEvent(newGuid, name));
        }

        public StubSubAggregate()
        {
        }

        public static StubSubAggregate Create(string name)
        {
            return new StubSubAggregate(Guid.NewGuid(), name);
        }

        public void ChangeName(string name)
        {
            Emit(new NameChangedEvent(AggregateId, name));
        }

        public void DoSomethingWithoutEventSubscription()
        {
            Emit(new NotRegisteredEvent(AggregateId));
        }

        public void Relationship(Guid relatedId)
        {
            Emit(new StubAggregateRelatedEvent(AggregateId, relatedId));
        }

        protected override void RegisterEvents()
        {
            SubscribeTo<NameChangedEvent>(x => { Name = x.Name; });
            SubscribeTo<StubAggregateCreatedEvent>(x =>
            {
                AggregateId = x.AggregateId;
                Name = x.Name;
            });

            SubscribeTo<StubAggregateRelatedEvent>(x =>
            {
                RelatedId = x.StubAggregateId;
            });
        }

    }
}