using System;
using Cars.Events;

namespace Cars.UnitTests.MessageBus.Stubs
{
    public class OrderedTestEvent : DomainEvent
    {
        public int Order { get; }

        public OrderedTestEvent(Guid streamId, int order) : base(streamId)
        {
            Order = order;
        }
    }
}