using System;
using EnjoyCQRS.Events;

namespace EnjoyCQRS.Testing.Shared.StubApplication.Domain.FooAggregate
{
    public class DidSomething : DomainEvent
    {
        public DidSomething(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}