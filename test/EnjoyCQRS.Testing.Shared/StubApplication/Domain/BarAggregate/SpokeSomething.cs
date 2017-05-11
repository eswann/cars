using System;
using EnjoyCQRS.Events;

namespace EnjoyCQRS.Testing.Shared.StubApplication.Domain.BarAggregate
{
    public class SpokeSomething : IDomainEvent
    {
        public string Text { get; }

        public SpokeSomething(string text)
        {
            Text = text;
        }

        public Guid AggregateId { get; } = Guid.NewGuid();
    }
}