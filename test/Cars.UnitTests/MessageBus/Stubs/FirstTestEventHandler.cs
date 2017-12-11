using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cars.Events;
using Cars.Handlers;

namespace Cars.UnitTests.MessageBus.Stubs
{
    public class FirstTestEventHandler : IEventHandler<TestEvent>
    {
        public List<Guid> Ids { get; } = new List<Guid>();

        public Task ExecuteAsync(TestEvent @event)
        {
            Ids.Add(@event.AggregateId);

            return Task.CompletedTask;
        }
    }
}