using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cars.Events;
using Cars.EventSource.SerializedEvents;
using Cars.EventSource.Storage;
using Cars.MessageBus;
using Cars.Testing.Shared;
using Cars.UnitTests.Domain.Stubs;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cars.UnitTests.Storage
{
    public class EventStoreTests
    {
        private const string _categoryName = "Unit";
        private const string _categoryValue = "Event store";

        private readonly InMemoryEventStore _inMemoryDomainEventStore;
        private readonly ISession _session;
        private readonly IRepository _repository;
        private readonly Mock<IEventPublisher> _mockEventPublisher;

        public EventStoreTests()
        {
            var eventSerializer = new EventSerializer(new JsonTextSerializer());

            _inMemoryDomainEventStore = new InMemoryEventStore();
            

            _mockEventPublisher = new Mock<IEventPublisher>();
            _mockEventPublisher.Setup(e => e.EnqueueAsync(It.IsAny<IEnumerable<IDomainEvent>>())).Returns(Task.CompletedTask);
            
            var loggerFactory = new LoggerFactory();
            var session = new Session(loggerFactory, _inMemoryDomainEventStore, _mockEventPublisher.Object, eventSerializer);
            _repository = new Repository(loggerFactory, session);

            var unitOfWorkMock = new Mock<ISession>();
            unitOfWorkMock.Setup(e => e.CommitAsync())
                .Callback(async () =>
                {
                    await session.CommitAsync();
                })
                .Returns(Task.CompletedTask);

            _session = unitOfWorkMock.Object;
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task When_calling_Save_it_will_add_the_domain_events_to_the_domain_event_storage()
        {
            var testAggregate = StubAggregate.Create("Walter White");
            testAggregate.ChangeName("Heinsenberg");

            _repository.Add(testAggregate);

            await _session.CommitAsync();

            var events = await _inMemoryDomainEventStore.GetEventsByAggregateId(testAggregate.AggregateId);
            
            events.Count().Should().Be(2);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task When_Save_Then_the_uncommited_events_should_be_published()
        {
            var testAggregate = StubAggregate.Create("Walter White");
            testAggregate.ChangeName("Heinsenberg");

            _repository.Add(testAggregate);
            await _session.CommitAsync();

            _mockEventPublisher.Verify(e => e.EnqueueAsync(It.IsAny<IEnumerable<IDomainEvent>>()));
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task When_load_stream_should_be_correct_version()
        {
            var testAggregate = StubAggregate.Create("Walter White");
            testAggregate.ChangeName("Heinsenberg");

            _repository.Add(testAggregate);
            await _session.CommitAsync();

            var testAggregate2 = await _repository.GetByIdAsync<StubAggregate>(testAggregate.AggregateId);
            
            testAggregate.Version.Should().Be(testAggregate2.Version);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Aggregate_stream_is_loaded_into_subAggregate()
        {
            var testAggregate = StubAggregate.Create("Walter White");
            testAggregate.ChangeName("Heinsenberg");

            _repository.Add(testAggregate);
            await _session.CommitAsync();

            var testSubAggregate = await _repository.GetByIdAsync<StubSubAggregate>(testAggregate.AggregateId);

            testSubAggregate.Version.Should().Be(testAggregate.Version);
            testSubAggregate.Name.Should().Be(testAggregate.Name);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task SubAggregate_updates_are_reflected_in_aggregate()
        {
            var testAggregate = StubAggregate.Create("Walter White");
            testAggregate.ChangeName("Heinsenberg");

            _repository.Add(testAggregate);
            await _session.CommitAsync();

            var testSubAggregate = await _repository.GetByIdAsync<StubSubAggregate>(testAggregate.AggregateId);
            testSubAggregate.ChangeName("Schrodinger");

            _repository.Add(testSubAggregate);
            await _session.CommitAsync();

            testAggregate = await _repository.GetByIdAsync<StubAggregate>(testAggregate.AggregateId);

            testAggregate.Name.Should().Be("Schrodinger");
        }
    }
}