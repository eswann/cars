using System;
using System.Linq;
using System.Threading.Tasks;
using Cars.Core;
using Cars.EventSource.Projections;
using Cars.EventStore.MongoDB;
using Cars.Testing.Shared.EventStore;
using Cars.Testing.Shared.StubApplication.Domain.Bar.Projections;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Xunit;

namespace Cars.MongoDB.IntegrationTests
{
    public class When_using_event_store : IDisposable
    {
        private static readonly ITextSerializer _bsonSerializer = new BsonTextSerializer();

        public const string CategoryName = "Integration";
        public const string CategoryValue = "MongoDB";
        public const string DatabaseName = "EventStore";

	    private readonly IMongoEventStoreSettings _defaultSettings = new MongoEventStoreSettings();

        private readonly MongoClient _mongoClient;

        static When_using_event_store()
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

        public When_using_event_store()
        {
            if (string.IsNullOrWhiteSpace(Test_Settings.MongoHost)) throw new NullReferenceException("The variable 'MONGODB_HOST' was not configured.");

            _mongoClient = new MongoClient($"mongodb://{Test_Settings.MongoHost}");

            _mongoClient.DropDatabase(DatabaseName);
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
        public async Task Events_are_stored()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore, new ProjectionSerializer(_bsonSerializer));

                var stream = await eventStoreTestSuit.EventTestsAsync();

                using (var projectionRepository = new MongoProjectionRepository(_mongoClient, _defaultSettings))
                {
                    var projection = await projectionRepository.GetAsync<BarProjection>(nameof(BarProjection), stream.AggregateId);

                    projection.Id.Should().Be(stream.AggregateId);
                    projection.LastText.Should().Be(stream.LastText);
                    projection.UpdatedAt.ToString("G").Should().Be(stream.UpdatedAt.ToString("G"));
                    projection.Messages.Count.Should().Be(stream.Messages.Count);
                }

                using (var projectionRepository = new MongoProjectionRepository<BarProjection>(_mongoClient, _defaultSettings))
                {
                    var projections = await projectionRepository.FindAsync(p => p.Id == stream.AggregateId);

                    projections.Count().Should().BeGreaterOrEqualTo(1);
                }
            }
        }
        
        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Snapshot_is_stored()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore, new ProjectionSerializer(_bsonSerializer));

                await eventStoreTestSuit.SnapshotTestsAsync();
            }
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task When_any_exception_is_thrown()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore, new ProjectionSerializer(_bsonSerializer));

                await eventStoreTestSuit.DoSomeProblemAsync();
            }
        }

        public void Dispose()
        {
            _mongoClient.DropDatabase(DatabaseName);
        }
    }
}