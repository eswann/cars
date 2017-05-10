using System;
using EnjoyCQRS.Commands;
using EnjoyCQRS.Events;

namespace EnjoyCQRS.Grace.UnitTests.Stubs
{
    public class TestEvent : DomainEvent
    {
        public TestEvent(Guid aggregateId, string someProperty) : base(aggregateId)
        {
            SomeProperty = someProperty;
        }

        public string SomeProperty { get; }

        public bool WasHandled { get; set; }
    }
}