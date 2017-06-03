using System;
using Cars.Events;

namespace Cars.UnitTests.Domain.Stubs.Events
{
    public class ChildCreatedEvent : DomainEvent
    {
        public Guid EntityId { get; }
        public string Name { get; }

        public ChildCreatedEvent(Guid streamId, Guid entityId, string name) : base(streamId)
        {
            EntityId = entityId;
            Name = name;
        }
    }
}