using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cars.Core;
using Cars.EventSource.Projections;
using Cars.EventStore.MongoDB;
using Cars.Testing.Shared.EventStore;
using Cars.Testing.Shared.StubApplication.Domain.BarAggregate.Projections;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Xunit;

namespace Cars.MongoDB.IntegrationTests.EventStore
{
    public class MongoEventStoreTests : IDisposable
    {
        private static readonly ITextSerializer _bsonSerializer = new BsonTextSerializer();

        public const string CategoryName = "Integration";
        public const string CategoryValue = "MongoDB";
        public const string DatabaseName = "cars";

	    private readonly IMongoEventStoreSettings _defaultSettings = new MongoEventStoreSettings {Database = DatabaseName};

        private readonly MongoClient _mongoClient;

        static MongoEventStoreTests()
        {
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register("camelCase", pack, t => true);

            BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);

            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        }

        public MongoEventStoreTests()
        {
            var mongoHost = Environment.GetEnvironmentVariable("MONGODB_HOST");

            if (string.IsNullOrWhiteSpace(mongoHost)) throw new ArgumentNullException("The variable 'MONGODB_HOST' was not configured.");

            _mongoClient = new MongoClient($"mongodb://{mongoHost}");

            _mongoClient.DropDatabase(DatabaseName);
        }

        [Trait(CategoryName, CategoryValue)]
        [Theory, MemberData(nameof(InvalidStates))]
        public void Should_validate_constructor_parameters(MongoClient mongoClient, MongoEventStoreSettings settings)
        {
            Action action = () => new MongoEventStore(mongoClient, settings);

            action.ShouldThrowExactly<ArgumentNullException>();
        }
        

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_use_default_settings()
        {

            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                eventStore.Settings.EventsCollectionName.Should().Be(_defaultSettings.EventsCollectionName);
                eventStore.Settings.SnapshotsCollectionName.Should().Be(_defaultSettings.SnapshotsCollectionName);
            }
        }

        [Trait(CategoryName, CategoryValue)]
        [Theory]
        [InlineData("events", null)]
        [InlineData(null, "snapshots")]
        public void Should_validate_settings(string eventCollectionName, string snapshotCollectionName)
        {
            var defaultSettings = new MongoEventStoreSettings
            {
                EventsCollectionName = eventCollectionName,
                SnapshotsCollectionName = snapshotCollectionName,
				Database = DatabaseName
            };
            
            Action action = () => new MongoEventStore(new MongoClient(), defaultSettings);

            action.ShouldThrowExactly<ArgumentNullException>();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_use_custom_settings()
        {
            var customSettings = new MongoEventStoreSettings
            {
                EventsCollectionName = "MyEvents",
                SnapshotsCollectionName = "MySnapshots",
				Database = DatabaseName
            };

            using (var eventStore = new MongoEventStore(_mongoClient, customSettings))
            {
                eventStore.Settings.EventsCollectionName.Should().Be(customSettings.EventsCollectionName);
                eventStore.Settings.SnapshotsCollectionName.Should().Be(customSettings.SnapshotsCollectionName);
            }
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_create_database()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var database = eventStore.Client.GetDatabase(DatabaseName);

                database.Should().NotBeNull();

                eventStore.Settings.Database.Should().Be(DatabaseName);
            }
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Test_events()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore, new ProjectionSerializer(_bsonSerializer));

                var aggregate = await eventStoreTestSuit.EventTestsAsync();

                using (var projectionRepository = new MongoProjectionRepository(_mongoClient, _defaultSettings))
                {
                    var projection = await projectionRepository.GetAsync<BarProjection>(nameof(BarProjection), aggregate.Id);

                    projection.Id.Should().Be(aggregate.Id);
                    projection.LastText.Should().Be(aggregate.LastText);
                    projection.UpdatedAt.ToString("G").Should().Be(aggregate.UpdatedAt.ToString("G"));
                    projection.Messages.Count.Should().Be(aggregate.Messages.Count);
                }

                using (var projectionRepository = new MongoProjectionRepository<BarProjection>(_mongoClient, _defaultSettings))
                {
                    var projections = await projectionRepository.FindAsync(e => e.Id == aggregate.Id);

                    projections.Count().Should().BeGreaterOrEqualTo(1);
                }
            }
        }
        
        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Test_snapshot()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore, new ProjectionSerializer(_bsonSerializer));

                await eventStoreTestSuit.SnapshotTestsAsync();
            }
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task When_any_exception_be_thrown()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore, new ProjectionSerializer(_bsonSerializer));

                await eventStoreTestSuit.DoSomeProblemAsync();
            }
        }

        public static IEnumerable<object[]> InvalidStates => new[]
        {
            new object[] { null, new MongoEventStoreSettings() },
            new object[] { new MongoClient(), null }
        };

        public void Dispose()
        {
            _mongoClient.DropDatabase(DatabaseName);
        }
    }
}