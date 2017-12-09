using System;
using System.Threading.Tasks;
using Cars.EventStore.MongoDB;
using Cars.Testing.Shared.EventStore;
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
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore);

                await eventStoreTestSuit.EventTestsAsync();
            }
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task SubAggregate_events_are_stored_with_primary_aggregate()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore);

                await eventStoreTestSuit.SubAggregateEventTestsAsync();
            }
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task When_any_exception_is_thrown()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore);

                await eventStoreTestSuit.DoSomeProblemAsync();
            }
        }

        public void Dispose()
        {
            _mongoClient.DropDatabase(DatabaseName);
        }
    }
}