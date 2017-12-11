using System;
using Cars.Events;

namespace Cars.Testing.Shared.StubApplication.EventHandlers
{
    public class ManyDependenciesEvent : DomainEvent
    {
        public string Text { get; }

        public ManyDependenciesEvent(string text)
        {
            AggregateId = Guid.NewGuid();
            Text = text;
        }
    }
}