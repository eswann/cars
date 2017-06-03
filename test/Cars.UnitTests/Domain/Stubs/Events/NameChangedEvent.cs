using System;
using Cars.Events;

namespace Cars.UnitTests.Domain.Stubs.Events
{
    public class NameChangedEvent : DomainEvent
    {
        public string Name { get; }

        public NameChangedEvent(Guid streamId, string name) : base(streamId)
        {
            Name = name;
        }
    }
}