using System;
using Cars.Events;

namespace Cars.MongoDB.IntegrationTests.Denormalizers
{
    public class ThingHappened : DomainEvent
    {
        public ThingHappened()
        {
        }

        public ThingHappened(string text)
        {
            AggregateId = Guid.NewGuid();
            Text = text;
        }

        public string Text { get; protected set; }

    }
}
