using System;
using System.Threading.Tasks;
using Cars.EventStore.MongoDB;
using Cars.Testing.Shared.EventStore;
using FluentAssertions;
using MongoDB.Driver;
using Xunit;

namespace Cars.MongoDB.IntegrationTests
{
    public class When_using_event_store : IDisposable
    {
        private const string _categoryName = "Integration";
        private const string _categoryValue = "MongoDB";
        private const string _databaseName = "EventStoreTests";

        private readonly IMongoEventStoreSettings _defaultSettings = new MongoEventStoreSettings { Database = _databaseName };
        private readonly MongoClient _mongoClient;

        public When_using_event_store()
        {
            if (string.IsNullOrWhiteSpace(TestSettings.MongoHost)) throw new NullReferenceException("The variable 'MONGODB_HOST' was not configured.");

            _mongoClient = new MongoClient($"mongodb://{TestSettings.MongoHost}");

            _mongoClient.DropDatabase(_databaseName);
        }


        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Should_create_database()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var database = eventStore.Client.GetDatabase(_databaseName);

                database.Should().NotBeNull();

                eventStore.Settings.Database.Should().Be(_databaseName);
            }
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Events_are_stored()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore);

                await eventStoreTestSuit.EventTestsAsync();
            }
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task SubAggregate_events_are_stored_with_primary_aggregate()
        {
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var eventStoreTestSuit = new EventStoreTestSuit(eventStore);

                await eventStoreTestSuit.SubAggregateEventTestsAsync();
            }
        }

        [Trait(_categoryName, _categoryValue)]
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
            _mongoClient.DropDatabase(_databaseName);
        }
    }
}