using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cars.Core;
using Cars.Events;
using Cars.EventSource;
using Cars.EventSource.Exceptions;
using Cars.EventSource.Projections;
using Cars.EventSource.Snapshots;
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
        private const string CategoryName = "Unit";
        private const string CategoryValue = "Session";

        private readonly Func<IEventStore, IEventPublisher, ISnapshotStrategy, Session> _sessionFactory = (eventStore, eventPublisher, snapshotStrategy) =>
        {
            var eventSerializer = CreateEventSerializer();
            var snapshotSerializer = CreateSnapshotSerializer();
            var projectionSerializer = CreateProjectionSerializer();

            var session = new Session(new TestLoggerFactory(), eventStore, eventPublisher, eventSerializer, snapshotSerializer, projectionSerializer, null, null, null, snapshotStrategy);

            return session;
        };

        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly JsonTextSerializer _textSerializer;

        public SessionTests()
        {
            _eventPublisherMock = new Mock<IEventPublisher>();

            _textSerializer = new JsonTextSerializer();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Cannot_pass_null_instance_of_LoggerFactory()
        {
            var eventPublisher = Mock.Of<IEventPublisher>();
            var eventStore = Mock.Of<IEventStore>();
            var eventSerializer = Mock.Of<IEventSerializer>();
            var snapshotSerializer = Mock.Of<ISnapshotSerializer>();
            var projectionSerializer = Mock.Of<IProjectionSerializer>();
            var projectionProviderScanner = Mock.Of<IProjectionProviderScanner>();
            var eventUpdateManager = Mock.Of<IEventUpdateManager>();
            var metadataProviders = Mock.Of<IEnumerable<IMetadataProvider>>();

            Action act = () => new Session(null, eventStore, eventPublisher, eventSerializer, snapshotSerializer, projectionSerializer, projectionProviderScanner, eventUpdateManager, metadataProviders);

            act.ShouldThrowExactly<ArgumentNullException>();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Cannot_pass_null_instance_of_EventStore()
        {
            var eventSerializer = Mock.Of<IEventSerializer>();
            var snapshotSerializer = Mock.Of<ISnapshotSerializer>();
            var projectionSerializer = Mock.Of<IProjectionSerializer>();
            var projectionProviderScanner = Mock.Of<IProjectionProviderScanner>();
            var eventPublisher = Mock.Of<IEventPublisher>();
            var eventUpdateManager = Mock.Of<IEventUpdateManager>();
            var metadataProviders = Mock.Of<IEnumerable<IMetadataProvider>>();

            Action act = () => new Session(new TestLoggerFactory(), null, eventPublisher, eventSerializer, snapshotSerializer, projectionSerializer, projectionProviderScanner, eventUpdateManager, metadataProviders);

            act.ShouldThrowExactly<ArgumentNullException>();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Cannot_pass_null_instance_of_EventPublisher()
        {
            var eventStore = Mock.Of<IEventStore>();
            var eventSerializer = Mock.Of<IEventSerializer>();
            var snapshotSerializer = Mock.Of<ISnapshotSerializer>();
            var projectionSerializer = Mock.Of<IProjectionSerializer>();
            var projectionProviderScanner = Mock.Of<IProjectionProviderScanner>();
            var eventUpdateManager = Mock.Of<IEventUpdateManager>();
            var metadataProviders = Mock.Of<IEnumerable<IMetadataProvider>>();

            Action act = () => new Session(new TestLoggerFactory(), eventStore, null, eventSerializer, snapshotSerializer, projectionSerializer, projectionProviderScanner, eventUpdateManager, metadataProviders);

            act.ShouldThrowExactly<ArgumentNullException>();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_throws_exception_When_stream_version_is_wrong()
        {
            var eventStore = new InMemoryEventStore();

            // create first session instance
            var session = _sessionFactory(eventStore, _eventPublisherMock.Object, null);

            var stubStream1 = StubAggregate.Create("Walter White");

            await session.AddAsync(stubStream1).ConfigureAwait(false);

            await session.SaveChangesAsync().ConfigureAwait(false);

            stubStream1.ChangeName("Going to Version 2. Expected Version 1.");

            // create second session instance to getting clear tracking
            session = _sessionFactory(eventStore, _eventPublisherMock.Object, null);

            var stubStream2 = await session.GetByIdAsync<StubAggregate>(stubStream1.AggregateId).ConfigureAwait(false);

            stubStream2.ChangeName("Going to Version 2");

            await session.AddAsync(stubStream2).ConfigureAwait(false);
            await session.SaveChangesAsync().ConfigureAwait(false);

            Func<Task> wrongVersion = async () => await session.AddAsync(stubStream1);

            wrongVersion.ShouldThrowExactly<ExpectedVersionException<StubAggregate>>().And.Mutator.Should().Be(stubStream1);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_retrieve_the_stream_from_tracking()
        {
            var eventStore = new InMemoryEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object, null);

            var stubStream1 = StubAggregate.Create("Walter White");

            await session.AddAsync(stubStream1).ConfigureAwait(false);

            await session.SaveChangesAsync().ConfigureAwait(false);

            stubStream1.ChangeName("Changes");

            var stubStream2 = await session.GetByIdAsync<StubAggregate>(stubStream1.AggregateId).ConfigureAwait(false);

            stubStream2.ChangeName("More changes");

            stubStream1.Should().BeSameAs(stubStream2);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_publish_in_correct_order()
        {
            var events = new List<IDomainEvent>();

            var eventStore = new InMemoryEventStore();

            _eventPublisherMock.Setup(e => e.PublishAsync(It.IsAny<IEnumerable<IDomainEvent>>())).Callback<IEnumerable<IDomainEvent>>(evts => events.AddRange(evts)).Returns(Task.CompletedTask);

            _eventPublisherMock.Setup(e => e.CommitAsync()).Returns(Task.CompletedTask);

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object, null);

            var stubStream1 = StubAggregate.Create("Walter White");
            var stubStream2 = StubAggregate.Create("Heinsenberg");

            stubStream1.ChangeName("Saul Goodman");

            stubStream2.Relationship(stubStream1.AggregateId);

            stubStream1.ChangeName("Jesse Pinkman");

            await session.AddAsync(stubStream1).ConfigureAwait(false);
            await session.AddAsync(stubStream2).ConfigureAwait(false);

            await session.SaveChangesAsync().ConfigureAwait(false);

            events[0].Should().BeOfType<StubStreamCreatedEvent>().Which.AggregateId.Should().Be(stubStream1.AggregateId);
            events[0].Should().BeOfType<StubStreamCreatedEvent>().Which.Name.Should().Be("Walter White");

            events[1].Should().BeOfType<StubStreamCreatedEvent>().Which.AggregateId.Should().Be(stubStream2.AggregateId);
            events[1].Should().BeOfType<StubStreamCreatedEvent>().Which.Name.Should().Be("Heinsenberg");

            events[2].Should().BeOfType<NameChangedEvent>().Which.AggregateId.Should().Be(stubStream1.AggregateId);
            events[2].Should().BeOfType<NameChangedEvent>().Which.Name.Should().Be("Saul Goodman");

            events[3].Should().BeOfType<StubStreamRelatedEvent>().Which.AggregateId.Should().Be(stubStream2.AggregateId);
            events[3].Should().BeOfType<StubStreamRelatedEvent>().Which.StubAggregateId.Should().Be(stubStream1.AggregateId);

            events[4].Should().BeOfType<NameChangedEvent>().Which.AggregateId.Should().Be(stubStream1.AggregateId);
            events[4].Should().BeOfType<NameChangedEvent>().Which.Name.Should().Be("Jesse Pinkman");
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task When_call_SaveChanges_Should_store_the_snapshot()
        {
            // Arrange

            var snapshotStrategy = CreateSnapshotStrategy();

            var eventStore = new StubEventStore();
            var session = _sessionFactory(eventStore, _eventPublisherMock.Object, snapshotStrategy);

            var stubStream = StubSnapshotAggregate.Create("Snap");

            stubStream.AddEntity("Child 1");
            stubStream.AddEntity("Child 2");

            await session.AddAsync(stubStream).ConfigureAwait(false);

            // Act

            await session.SaveChangesAsync().ConfigureAwait(false);

            // Assert

            eventStore.SaveSnapshotMethodCalled.Should().BeTrue();

            var commitedSnapshot = eventStore.Snapshots.First(e => e.AggregateId == stubStream.AggregateId);

            commitedSnapshot.Should().NotBeNull();

            var metadata = (IMetadata)_textSerializer.Deserialize<EventSource.Metadata>(commitedSnapshot.SerializedMetadata);

            var snapshotClrType = metadata.GetValue(MetadataKeys.SnapshotClrType, value => value.ToString());

            Type.GetType(snapshotClrType).Name.Should().Be(typeof(StubSnapshotStreamSnapshot).Name);

            var snapshot = _textSerializer.Deserialize<StubSnapshotStreamSnapshot>(commitedSnapshot.SerializedData);

            snapshot.Name.Should().Be(stubStream.Name);
            snapshot.SimpleEntities.Count.Should().Be(stubStream.Entities.Count);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_restore_stream_using_snapshot()
        {
            var snapshotStrategy = CreateSnapshotStrategy();

            var eventStore = new StubEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object, snapshotStrategy);

            var stubStream = StubSnapshotAggregate.Create("Snap");

            stubStream.AddEntity("Child 1");
            stubStream.AddEntity("Child 2");

            await session.AddAsync(stubStream).ConfigureAwait(false);
            await session.SaveChangesAsync().ConfigureAwait(false);

            session = _sessionFactory(eventStore, _eventPublisherMock.Object, snapshotStrategy);

            var stream = await session.GetByIdAsync<StubSnapshotAggregate>(stubStream.AggregateId).ConfigureAwait(false);

            eventStore.GetSnapshotMethodCalled.Should().BeTrue();

            stream.Version.Should().Be(3);
            stream.AggregateId.Should().Be(stubStream.AggregateId);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task When_not_exists_snapshot_yet_Then_stream_should_be_constructed_using_your_events()
        {
            var snapshotStrategy = CreateSnapshotStrategy(false);

            var eventStore = new StubEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object, snapshotStrategy);

            var stubStream = StubSnapshotAggregate.Create("Snap");

            stubStream.AddEntity("Child 1");
            stubStream.AddEntity("Child 2");

            await session.AddAsync(stubStream).ConfigureAwait(false);
            await session.SaveChangesAsync().ConfigureAwait(false);

            session = _sessionFactory(eventStore, _eventPublisherMock.Object, snapshotStrategy);

            var stream = await session.GetByIdAsync<StubSnapshotAggregate>(stubStream.AggregateId).ConfigureAwait(false);

            stream.Name.Should().Be(stubStream.Name);
            stream.Entities.Count.Should().Be(stubStream.Entities.Count);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Getting_snapshot_and_forward_events()
        {
            var snapshotStrategy = CreateSnapshotStrategy();

            var eventStore = new StubEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object, snapshotStrategy);

            var stubStream = StubSnapshotAggregate.Create("Snap");

            await session.AddAsync(stubStream).ConfigureAwait(false);
            await session.SaveChangesAsync().ConfigureAwait(false); // Version 1

            stubStream.ChangeName("Renamed");
            stubStream.ChangeName("Renamed again");

            // dont make snapshot
            snapshotStrategy = CreateSnapshotStrategy(false);

            session = _sessionFactory(eventStore, _eventPublisherMock.Object, snapshotStrategy);

            await session.AddAsync(stubStream).ConfigureAwait(false);
            await session.SaveChangesAsync().ConfigureAwait(false); // Version 3

            var stubStreamFromSnapshot = await session.GetByIdAsync<StubSnapshotAggregate>(stubStream.AggregateId).ConfigureAwait(false);

            stubStreamFromSnapshot.Version.Should().Be(3);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public Task Should_throws_exception_When_stream_was_not_found()
        {
            var snapshotStrategy = CreateSnapshotStrategy();

            var eventStore = new StubEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object, snapshotStrategy);

            var newId = Guid.NewGuid();

            Func<Task> act = async () =>
            {
                await session.GetByIdAsync<StubAggregate>(newId).ConfigureAwait(false);
            };

            var assertion = act.ShouldThrowExactly<StreamNotFoundException>();

            assertion.Which.StreamName.Should().Be(typeof(StubAggregate).Name);
            assertion.Which.AggregateId.Should().Be(newId);

            return Task.CompletedTask;
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_internal_rollback_When_exception_was_throw_on_saving()
        {
            var snapshotStrategy = CreateSnapshotStrategy(false);

            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.Setup(e => e.SaveAsync(It.IsAny<IEnumerable<ISerializedEvent>>()))
                .Callback(DoThrowExcetion)
                .Returns(Task.CompletedTask);

            var session = _sessionFactory(eventStoreMock.Object, _eventPublisherMock.Object, snapshotStrategy);

            var stubStream = StubAggregate.Create("Guilty");

            await session.AddAsync(stubStream).ConfigureAwait(false);

            try
            {
                await session.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                eventStoreMock.Verify(e => e.Rollback(), Times.Once);
                _eventPublisherMock.Verify(e => e.Rollback(), Times.Once);
            }
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_manual_rollback_When_exception_was_throw_on_saving()
        {
            var snapshotStrategy = CreateSnapshotStrategy(false);

            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.Setup(e => e.SaveAsync(It.IsAny<IEnumerable<ISerializedEvent>>()))
                .Callback(DoThrowExcetion)
                .Returns(Task.CompletedTask);

            eventStoreMock.Setup(e => e.BeginTransaction());
            eventStoreMock.Setup(e => e.Rollback());

            var session = _sessionFactory(eventStoreMock.Object, _eventPublisherMock.Object, snapshotStrategy);

            var stubStream = StubAggregate.Create("Guilty");

            session.BeginTransaction();

            eventStoreMock.Verify(e => e.BeginTransaction(), Times.Once);

            await session.AddAsync(stubStream).ConfigureAwait(false);

            try
            {
                await session.SaveChangesAsync().ConfigureAwait(false);
            }

            catch (Exception)
            {
                session.Rollback();
            }

            eventStoreMock.Verify(e => e.Rollback(), Times.Once);
            _eventPublisherMock.Verify(e => e.Rollback(), Times.Once);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Cannot_BeginTransaction_twice()
        {
            var snapshotStrategy = CreateSnapshotStrategy(false);

            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.SetupAllProperties();

            var session = _sessionFactory(eventStoreMock.Object, _eventPublisherMock.Object, snapshotStrategy);

            session.BeginTransaction();

            Action act = () => session.BeginTransaction();

            act.ShouldThrowExactly<InvalidOperationException>();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task When_occur_error_on_publishing_Then_rollback_should_be_called()
        {
            var snapshotStrategy = CreateSnapshotStrategy(false);

            _eventPublisherMock.Setup(e => e.PublishAsync(It.IsAny<IDomainEvent>()))
                .Callback(DoThrowExcetion)
                .Returns(Task.CompletedTask);

            _eventPublisherMock.Setup(e => e.PublishAsync(It.IsAny<IEnumerable<IDomainEvent>>()))
                .Callback(DoThrowExcetion)
                .Returns(Task.CompletedTask);

            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.SetupAllProperties();

            var session = _sessionFactory(eventStoreMock.Object, _eventPublisherMock.Object, snapshotStrategy);

            await session.AddAsync(StubAggregate.Create("Test"));

            try
            {
                await session.SaveChangesAsync();
            }
            catch (Exception)
            {
                eventStoreMock.Verify(e => e.Rollback(), Times.Once);
                _eventPublisherMock.Verify(e => e.Rollback(), Times.Once);
            }
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task When_occur_error_on_Save_in_EventStore_Then_rollback_should_be_called()
        {
            var snapshotStrategy = CreateSnapshotStrategy(false);

            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.SetupAllProperties();
            eventStoreMock.Setup(e => e.SaveAsync(It.IsAny<IEnumerable<ISerializedEvent>>()))
                .Callback(DoThrowExcetion)
                .Returns(Task.CompletedTask);

            var session = _sessionFactory(eventStoreMock.Object, _eventPublisherMock.Object, snapshotStrategy);

            await session.AddAsync(StubAggregate.Create("Test")).ConfigureAwait(false);

            try
            {
                await session.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                eventStoreMock.Verify(e => e.Rollback(), Times.Once);
                _eventPublisherMock.Verify(e => e.Rollback(), Times.Once);
            }
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task When_Save_events_Then_stream_projection_should_be_created()
        {
            var snapshotStrategy = CreateSnapshotStrategy(false);

            var eventStore = new StubEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object, snapshotStrategy);

            var stubStream1 = StubAggregate.Create("Walter White");

            stubStream1.ChangeName("Saul Goodman");
            stubStream1.ChangeName("Jesse Pinkman");

            await session.AddAsync(stubStream1).ConfigureAwait(false);

            await session.SaveChangesAsync().ConfigureAwait(false);

            var projectionKey = new InMemoryEventStore.ProjectionKey(stubStream1.AggregateId, typeof(StubStreamProjection).Name);

            eventStore.Projections.ContainsKey(projectionKey).Should().BeTrue();

            var projection = _textSerializer.Deserialize<StubStreamProjection>(eventStore.Projections[projectionKey].ToString());
            
            projection.Id.Should().Be(stubStream1.AggregateId);
            projection.Name.Should().Be(stubStream1.Name);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task When_Save_events_Then_stream_projection_should_be_updated()
        {
            var snapshotStrategy = CreateSnapshotStrategy(false);

            var eventStore = new StubEventStore();

            var session = _sessionFactory(eventStore, _eventPublisherMock.Object, snapshotStrategy);

            var stubStream1 = StubAggregate.Create("Walter White");

            await session.AddAsync(stubStream1).ConfigureAwait(false);
            await session.SaveChangesAsync().ConfigureAwait(false);

            stubStream1 = await session.GetByIdAsync<StubAggregate>(stubStream1.AggregateId).ConfigureAwait(false);

            stubStream1.ChangeName("Jesse Pinkman");

            await session.SaveChangesAsync().ConfigureAwait(false);

            eventStore.Projections.Count.Should().Be(1);

            var projectionKey = new InMemoryEventStore.ProjectionKey(stubStream1.AggregateId, typeof(StubStreamProjection).Name);
            eventStore.Projections.ContainsKey(projectionKey).Should().BeTrue();

            var projection = _textSerializer.Deserialize<StubStreamProjection>(eventStore.Projections[projectionKey].ToString());

            projection.Id.Should().Be(stubStream1.AggregateId);
            projection.Name.Should().Be(stubStream1.Name);
        }

        private static ISnapshotStrategy CreateSnapshotStrategy(bool makeSnapshot = true)
        {
            var snapshotStrategyMock = new Mock<ISnapshotStrategy>();
            snapshotStrategyMock.Setup(e => e.CheckSnapshotSupport(It.IsAny<Type>())).Returns(true);
            snapshotStrategyMock.Setup(e => e.ShouldMakeSnapshot(It.IsAny<IMutator>())).Returns(makeSnapshot);

            return snapshotStrategyMock.Object;
        }
        
        private static IEventSerializer CreateEventSerializer()
        {
            return new EventSerializer(new JsonTextSerializer());
        }

        private static ISnapshotSerializer CreateSnapshotSerializer()
        {
            return new SnapshotSerializer(new JsonTextSerializer());
        }

        private static IProjectionSerializer CreateProjectionSerializer()
        {
            return new ProjectionSerializer(new JsonTextSerializer());
        }

        private static void DoThrowExcetion()
        {
            throw new Exception("Sorry, this is my fault.");
        }
    }
}