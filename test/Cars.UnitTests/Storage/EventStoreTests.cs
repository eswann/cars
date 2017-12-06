using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cars.Events;
using Cars.EventSource;
using Cars.EventSource.SerializedEvents;
using Cars.EventSource.Snapshots;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository _repository;
        private readonly Mock<IEventPublisher> _mockEventPublisher;

        public EventStoreTests()
        {
            var eventSerializer = new EventSerializer(new JsonTextSerializer());
            var snapshotSerializer = new SnapshotSerializer(new JsonTextSerializer());

            _inMemoryDomainEventStore = new InMemoryEventStore();
            

            _mockEventPublisher = new Mock<IEventPublisher>();
            _mockEventPublisher.Setup(e => e.PublishAsync(It.IsAny<IEnumerable<IDomainEvent>>())).Returns(Task.CompletedTask);
            
            var loggerFactory = new LoggerFactory();
            var session = new Session(loggerFactory, _inMemoryDomainEventStore, _mockEventPublisher.Object, eventSerializer, snapshotSerializer);
            _repository = new Repository(loggerFactory, session);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(e => e.CommitAsync())
                .Callback(async () =>
                {
                    await session.SaveChangesAsync().ConfigureAwait(false);
                })
                .Returns(Task.CompletedTask);

            _unitOfWork = unitOfWorkMock.Object;
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task When_calling_Save_it_will_add_the_domain_events_to_the_domain_event_storage()
        {
            var testStream = StubAggregate.Create("Walter White");
            testStream.ChangeName("Heinsenberg");

            await _repository.AddAsync(testStream).ConfigureAwait(false);

            await _unitOfWork.CommitAsync().ConfigureAwait(false);

            var events = await _inMemoryDomainEventStore.GetAllEventsAsync(testStream.AggregateId);
            
            events.Count().Should().Be(2);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task When_Save_Then_the_uncommited_events_should_be_published()
        {
            var testStream = StubAggregate.Create("Walter White");
            testStream.ChangeName("Heinsenberg");

            await _repository.AddAsync(testStream).ConfigureAwait(false);
            await _unitOfWork.CommitAsync().ConfigureAwait(false);

            _mockEventPublisher.Verify(e => e.PublishAsync(It.IsAny<IEnumerable<IDomainEvent>>()));
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task When_load_stream_should_be_correct_version()
        {
            var testStream = StubAggregate.Create("Walter White");
            testStream.ChangeName("Heinsenberg");

            await _repository.AddAsync(testStream).ConfigureAwait(false);
            await _unitOfWork.CommitAsync().ConfigureAwait(false);

            var testStream2 = await _repository.GetByIdAsync<StubAggregate>(testStream.AggregateId).ConfigureAwait(false);
            
            testStream.Version.Should().Be(testStream2.Version);
        }
    }
}