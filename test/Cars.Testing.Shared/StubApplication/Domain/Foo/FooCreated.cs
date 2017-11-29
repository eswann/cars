using System;
using Cars.Events;

namespace Cars.Testing.Shared.StubApplication.Domain.Foo
{
    public class FooCreated : DomainEvent
    {
        public FooCreated(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}