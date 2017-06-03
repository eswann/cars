using System;
using Cars.Events;

namespace Cars.UnitTests.Domain.Stubs.Events
{
    public class StubStreamRelatedEvent : DomainEvent
    {
        public Guid StubStreamId { get; }

        public StubStreamRelatedEvent(Guid streamId, Guid stubStreamId) : base(streamId)
        {
            StubStreamId = stubStreamId;
        }
    }
}