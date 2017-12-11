using System;
using Cars.Events;

namespace Cars.Testing.Shared.StubApplication.Domain.Bar
{
    public class SpokeSomething : DomainEvent
    {
        public string Text { get; }

        public SpokeSomething(string text)
        {
            AggregateId = new Guid();
            Text = text;
        }
    }
}