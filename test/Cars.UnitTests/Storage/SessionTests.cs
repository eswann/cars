using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cars.Core;
using Cars.Events;
using Cars.EventSource;
using Cars.EventSource.Exceptions;
using Cars.EventSource.SerializedEvents;
using Cars.EventSource.Storage;
using Cars.MessageBus;
using Cars.Testing.Shared;
using Cars.Testing.Shared.Logging;
using Cars.UnitTests.Domain.Stubs;
using Cars.UnitTests.Domain.Stubs.Events;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cars.UnitTests.Storage
{
    public class SessionTests
    {
        private const string _categoryName = "Unit";
        private const string _categoryValue = "Session";

        private readonly Func<IEventStore, IEventPublisher, Session> _sessionFactory = (eventStore, eventPublisher) =>
        {
            var eventSerializer = CreateEventSerializer();
   
            var session = new Session(new TestLoggerFactory(), eventStore, eventPublisher, eventSerializer);

            return session;
        };

        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly JsonTextSerializer _textSerializer;

        public SessionTests()
        {
            _eventPublisherMock = new Mock<IEventPublisher>();

            _textSerializer = new JsonTextSerializer();
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Cannot_pass_null_instance_of_LoggerFactory()
        {
            var eventPublisher = Mock.Of<IEventPublisher>();
            var eventStore = Mock.Of<IEventStore>();
            var eventSerializer = Mock.Of<IEventSerializer>();
            var eventUpdateManager = Mock.Of<IEventUpdateManager>();
            var metadataProviders = Mock.Of<IEnumerable<IMetadataProvider>>();

            Action act = () => new Session(null, eventStore, eventPublisher, eventSerializer, eventUpdateManager, metadataProviders);

            act.ShouldThrowExactly<ArgumentNullException>();
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Cannot_pass_null_instance_of_EventStore()
        {
            var eventSerializer = Mock.Of<IEventSerializer>();
            var eventPublisher = Mock.Of<IEventPublisher>();
            var eventUpdateManager = Mock.Of<IEventUpdateManager>();
            var metadataProviders = Mock.Of<IEnumerable<IMetadataProvider>>();

            Action act = () => new Session(new TestLoggerFactory(), null, eventPublisher, eventSerializer, eventUpdateManager, metadataProviders);

            act.ShouldThrowExactly<ArgumentNullException>();
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Cannot_pass_null_instance_of_EventPublisher()
        {
            var eventStore = Mock.Of<IEventStore>();
            var eventSerializer = Mock.Of<IEventSerializer>();
            var eventUpdateManager = Mock.Of<IEventUpdateManager>();
            var metadataProviders = Mock.Of<IEnumerable<IMetadataProvider>>();

            Action act = () => new Session(new TestLoggerFactory(), eventStore, null, eventSerializer, eventUpdateManager, metadataProviders);

            act.ShouldThrowExactly<ArgumentNullException>();
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Should_throw_exception_when_different_representations_of_same_stream_have_changes()
        {
            var eventStore = new InMemoryEventStore();

            // create first session instance
            var session = _sessionFactory(eventStore, _eventPublisherMock.Object);

            var stubAggregate1 = StubAggregate.Create("Walter White");

            session.Add(stubAggregate1);

            await session.CommitAsync();

            stubAggregate1 = await session.GetByIdAsync<StubAggregate>(stubAggregate1.AggregateId);
            var stubAggregate2 = await session.GetByIdAsync<StubSubAggregate>(stubAggregate1.AggregateId);

            stubAggregate1.ChangeName("Heisenberg");
            stubAggregate2.ChangeName("Oppenheimer");

            await Assert.ThrowsAsync<AggregateConcurrencyException>(() => session.CommitAsync());
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Should_not_throw_exception_when_different_representations_of_same_stream_do_not_have_changes()
        {
            var eventStore = new InMemoryEventStore();

            // create first session instance
            var session = _sessionFactory(eventStore, _eventPublisherMock.Object);

            var stubAggregate1 = StubAggregate.Create("Walter White");

            session.Add(stubAggregate1);

            await session.CommitAsync();

            stubAggregate1 = await session.GetByIdAsync<StubAggregate>(stubAggregate1.AggregateId);
            await session.GetByIdAsync<StubSubAggregate>(stubAggregate1.AggregateId);

            stubAggregate1.ChangeName("Heisenberg");
            await session.CommitAsync();
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Should_not_retrieve_the_aggregate_from_tracking_after_commit()
        {
            var eventStore = new InMemoryEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object);

            var stubAggregate1 = StubAggregate.Create("Walter White");

            session.Add(stubAggregate1);

            await session.CommitAsync();

            var stubAggregate2 = await session.GetByIdAsync<StubAggregate>(stubAggregate1.AggregateId);

            stubAggregate1.Should().NotBeSameAs(stubAggregate2);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Should_retrieve_the_aggregate_from_tracking_for_subAggregate()
        {
            var eventStore = new InMemoryEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object);

            var stubAggregate1 = StubSubAggregate.Create("Walter White");

            session.Add(stubAggregate1);

            await session.CommitAsync();

            stubAggregate1.ChangeName("Changes");

            var stubAggregate2 = await session.GetByIdAsync<StubSubAggregate>(stubAggregate1.AggregateId);


            stubAggregate1.Should().NotBeSameAs(stubAggregate2);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task SubAggregate_should_write_to_root_aggregate()
        {
            var eventStore = new InMemoryEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object);

            var subAggregate = StubSubAggregate.Create("Walter White");

            session.Add(subAggregate);

            await session.CommitAsync();

            var rootAggregate = await session.GetByIdAsync<StubAggregate>(subAggregate.AggregateId);

            rootAggregate.Name.Should().Be(subAggregate.Name);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Should_publish_in_correct_order()
        {
            var events = new List<IDomainEvent>();

            var eventStore = new InMemoryEventStore();

            _eventPublisherMock.Setup(e => e.EnqueueAsync(It.IsAny<IEnumerable<IDomainEvent>>())).Callback<IEnumerable<IDomainEvent>>(evts => events.AddRange(evts)).Returns(Task.CompletedTask);

            _eventPublisherMock.Setup(e => e.CommitAsync()).Returns(Task.CompletedTask);

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object);

            var stubAggregate1 = StubAggregate.Create("Walter White");
            var stubAggregate2 = StubAggregate.Create("Heinsenberg");

            stubAggregate1.ChangeName("Saul Goodman");

            stubAggregate2.Relationship(stubAggregate1.AggregateId);

            stubAggregate1.ChangeName("Jesse Pinkman");

            session.Add(stubAggregate1);
            session.Add(stubAggregate2);

            await session.CommitAsync();

            events[0].Should().BeOfType<StubAggregateCreatedEvent>().Which.AggregateId.Should().Be(stubAggregate1.AggregateId);
            events[0].Should().BeOfType<StubAggregateCreatedEvent>().Which.Name.Should().Be("Walter White");

            events[1].Should().BeOfType<StubAggregateCreatedEvent>().Which.AggregateId.Should().Be(stubAggregate2.AggregateId);
            events[1].Should().BeOfType<StubAggregateCreatedEvent>().Which.Name.Should().Be("Heinsenberg");

            events[2].Should().BeOfType<NameChangedEvent>().Which.AggregateId.Should().Be(stubAggregate1.AggregateId);
            events[2].Should().BeOfType<NameChangedEvent>().Which.Name.Should().Be("Saul Goodman");

            events[3].Should().BeOfType<StubAggregateRelatedEvent>().Which.AggregateId.Should().Be(stubAggregate2.AggregateId);
            events[3].Should().BeOfType<StubAggregateRelatedEvent>().Which.StubAggregateId.Should().Be(stubAggregate1.AggregateId);

            events[4].Should().BeOfType<NameChangedEvent>().Which.AggregateId.Should().Be(stubAggregate1.AggregateId);
            events[4].Should().BeOfType<NameChangedEvent>().Which.Name.Should().Be("Jesse Pinkman");
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public Task Should_throw_exception_When_aggregate_was_not_found()
        {
            var eventStore = new InMemoryEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object);

            var newId = Guid.NewGuid();

            Func<Task> act = async () =>
            {
                await session.GetByIdAsync<StubAggregate>(newId);
            };

            var assertion = act.ShouldThrowExactly<AggregateNotFoundException>();

            assertion.Which.AggregateName.Should().Be(typeof(StubAggregate).Name);
            assertion.Which.AggregateId.Should().Be(newId);

            return Task.CompletedTask;
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Should_internal_rollback_When_exception_was_throw_on_saving()
        {
            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.Setup(e => e.SaveAsync(It.IsAny<IEnumerable<ISerializedEvent>>()))
                .Callback(DoThrowExcetion)
                .Returns(Task.CompletedTask);

            var session = _sessionFactory(eventStoreMock.Object, _eventPublisherMock.Object);

            var stubAggregate = StubAggregate.Create("Guilty");

            session.Add(stubAggregate);

            try
            {
                await session.CommitAsync();
            }
            catch (Exception)
            {
                eventStoreMock.Verify(e => e.Rollback(), Times.Once);
                _eventPublisherMock.Verify(e => e.Rollback(), Times.Once);
            }
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Cannot_BeginTransaction_twice()
        {
            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.SetupAllProperties();

            var session = _sessionFactory(eventStoreMock.Object, _eventPublisherMock.Object);

            session.BeginTransaction();

            Action act = () => session.BeginTransaction();

            act.ShouldThrowExactly<InvalidOperationException>();
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task When_occur_error_on_publishing_Then_rollback_should_be_called()
        {
            _eventPublisherMock.Setup(e => e.EnqueueAsync(It.IsAny<IDomainEvent>()))
                .Callback(DoThrowExcetion)
                .Returns(Task.CompletedTask);

            _eventPublisherMock.Setup(e => e.EnqueueAsync(It.IsAny<IEnumerable<IDomainEvent>>()))
                .Callback(DoThrowExcetion)
                .Returns(Task.CompletedTask);

            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.SetupAllProperties();

            var session = _sessionFactory(eventStoreMock.Object, _eventPublisherMock.Object);

            session.Add(StubAggregate.Create("Test"));

            try
            {
                await session.CommitAsync();
            }
            catch (Exception)
            {
                eventStoreMock.Verify(e => e.Rollback(), Times.Once);
                _eventPublisherMock.Verify(e => e.Rollback(), Times.Once);
            }
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task When_occur_error_on_Save_in_EventStore_Then_rollback_should_be_called()
        {
            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.SetupAllProperties();
            eventStoreMock.Setup(e => e.SaveAsync(It.IsAny<IEnumerable<ISerializedEvent>>()))
                .Callback(DoThrowExcetion)
                .Returns(Task.CompletedTask);

            var session = _sessionFactory(eventStoreMock.Object, _eventPublisherMock.Object);

            session.Add(StubAggregate.Create("Test"));

            try
            {
                await session.CommitAsync();
            }
            catch (Exception)
            {
                eventStoreMock.Verify(e => e.Rollback(), Times.Once);
                _eventPublisherMock.Verify(e => e.Rollback(), Times.Once);
            }
        }
        
        private static IEventSerializer CreateEventSerializer()
        {
            return new EventSerializer(new JsonTextSerializer());
        }

        private static void DoThrowExcetion()
        {
            throw new Exception("Sorry, this is my fault.");
        }
    }
}