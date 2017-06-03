using System;
using Cars.Events;

namespace Cars.Testing.Shared.StubApplication.Domain.Foo
{
    public class DidSomething : DomainEvent
    {
        public DidSomething(Guid streamId) : base(streamId)
        {
        }
    }
}