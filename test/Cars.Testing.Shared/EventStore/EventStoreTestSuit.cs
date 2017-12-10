using System;
using System.Threading.Tasks;
using Cars.EventSource.SerializedEvents;
using Cars.EventSource.Storage;
using Cars.MessageBus.InProcess;
using Cars.Testing.Shared.StubApplication.Domain.Bar;
using Cars.Testing.Shared.StubApplication.Domain.Foo;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cars.Testing.Shared.EventStore
{
    public class EventStoreTestSuit
    {
        private readonly EventStoreWrapper _eventStore;
        
        public EventStoreTestSuit(IEventStore eventStore)
        {
            _eventStore = new EventStoreWrapper(eventStore);
        }

        public async Task<Bar> EventTestsAsync()
        {
            var bar = GenerateBar();

            var session = CreateSession();

            session.Add(bar);
            await session.CommitAsync();

            session = CreateSession();

            var bar2 = await session.GetByIdAsync<Bar>(bar.AggregateId);

            var result = _eventStore.CalledMethods.HasFlag(EventStoreMethods.Ctor
                | EventStoreMethods.BeginTransaction
                | EventStoreMethods.SaveAsync
                | EventStoreMethods.CommitAsync
                | EventStoreMethods.GetAllEventsAsync);

            bar.AggregateId.Should().Be(bar2.AggregateId);

            result.Should().BeTrue();

            return bar;
        }

        public async Task<SubBar> SubAggregateEventTestsAsync()
        {
            var subBar = GenerateSubBar();

            var session = CreateSession();

            session.Add(subBar);
            await session.CommitAsync();

            session = CreateSession();

            var bar = await session.GetByIdAsync<Bar>(subBar.AggregateId);

            bar.AggregateId.Should().Be(subBar.AggregateId);
            bar.LastText.Should().Be(subBar.LastText);
            bar.Messages.Count.Should().Be(subBar.Messages.Count);

            return subBar;
        }

        public async Task DoSomeProblemAsync()
        {
            var foo = GenerateFoo();

            var session = CreateFaultSession();

            session.Add(foo);
            try
            {
                await session.CommitAsync();
            }
            catch (Exception)
            {
                var result = _eventStore.CalledMethods.HasFlag(EventStoreMethods.Ctor 
                    | EventStoreMethods.BeginTransaction 
                    | EventStoreMethods.SaveAsync 
                    | EventStoreMethods.Rollback);

                result.Should().BeTrue();
            }
        }

        private ISession CreateSession()
        {
            var session = new Session(new LoggerFactory(), _eventStore, new EventPublisher(StubEventRouter.Ok()), new EventSerializer(new JsonTextSerializer()));

            return session;
        }

        private ISession CreateFaultSession()
        {
            var faultSession = new Session(new LoggerFactory(), _eventStore, new EventPublisher(StubEventRouter.Fault()), new EventSerializer(new JsonTextSerializer()));

            return faultSession;
        }

        private static Foo GenerateFoo(int quantity = 10)
        {
            var foo = new Foo(Guid.NewGuid());

            for (var i = 0; i < quantity; i++)
            {
                foo.DoSomething();
            }

            return foo;
        }

        private static Bar GenerateBar(int quantity = 10)
        {
            var bar = Bar.Create(Guid.NewGuid());

            for (var i = 0; i < quantity; i++)
            {
                bar.Speak($"Hello number {i}.");
            }

            return bar;
        }

        private static SubBar GenerateSubBar(int quantity = 10)
        {
            var subBar = SubBar.Create(Guid.NewGuid());

            for (var i = 0; i < quantity; i++)
            {
                subBar.Speak($"Hello number {i}.");
            }

            return subBar;
        }
    }
}