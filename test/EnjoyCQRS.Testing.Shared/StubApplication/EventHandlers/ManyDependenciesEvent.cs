using System;
using EnjoyCQRS.Events;

namespace EnjoyCQRS.Testing.Shared.StubApplication.EventHandlers
{
    public class ManyDependenciesEvent : IDomainEvent
    {
        public string Text { get; }

        public ManyDependenciesEvent(string text)
        {
            Text = text;
        }

        public Guid AggregateId { get; } = Guid.NewGuid();
    }
}