using System;
using Cars.Events;

namespace Cars.UnitTests.Domain.Stubs.Events
{
    public class StubStreamRelatedEvent : DomainEvent
    {
        public Guid StubAggregateId { get; }

        public StubStreamRelatedEvent(Guid aggregateId, Guid stubAggregateId) : base(aggregateId)
        {
            StubAggregateId = stubAggregateId;
        }
    }
}