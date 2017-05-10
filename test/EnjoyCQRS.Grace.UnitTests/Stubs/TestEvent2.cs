using System;
using EnjoyCQRS.Commands;
using EnjoyCQRS.Events;

namespace EnjoyCQRS.Grace.UnitTests.Stubs
{
    public class TestEvent2 : DomainEvent
    {
        public TestEvent2(Guid aggregateId, string someProperty) : base(aggregateId)
        {
            SomeProperty = someProperty;
        }

        public string SomeProperty { get; }

        public bool WasHandled { get; set; }
    }
}