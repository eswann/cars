using System;
using System.Threading.Tasks;
using Cars.EventSource.SerializedEvents;
using Cars.EventSource.Storage;
using Cars.EventStore.MongoDB;
using Cars.EventStore.MongoDB.Projections;
using Cars.MessageBus.InProcess;
using Cars.Testing.Shared;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Xunit;

namespace Cars.MongoDB.IntegrationTests.Denormalizers
{
    public class When_replaying_denormalizers
    {
        private const string _databaseName = "EventStoreDenormalizerTests";

        private readonly IMongoEventStoreSettings _defaultSettings = new MongoEventStoreSettings { Database = _databaseName };
        private readonly MongoClient _mongoClient;

        public When_replaying_denormalizers()
        {
            _mongoClient = new MongoClient($"mongodb://{TestSettings.MongoHost}");

            _mongoClient.DropDatabase(_databaseName);

        }

        [Fact]
        public async Task Projection_is_rebuilt_by_denormalizer()
        {
            //Need to create an aggregate here.
            var serializer = new EventSerializer(new BsonTextSerializer());

            DenormAggregate aggregate;
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                aggregate = new DenormAggregate(Guid.NewGuid());
                for (int i = 0; i < 10; i++)
                {
                    aggregate.DoThing("Thing_" + i);
                }
                
                var session = new Session(new LoggerFactory(), eventStore, new EventPublisher(StubEventRouter.Ok()), serializer);
                session.Add(aggregate);
                await session.CommitAsync();
            }
            
            var projectionRepository = new MongoProjectionRepository(_mongoClient, _defaultSettings);
            using (var eventStore = new MongoEventStore(_mongoClient, _defaultSettings))
            {
                var testDenormalizer = new TestDenormalizer(projectionRepository, eventStore, serializer);
                await testDenormalizer.RebuildAsync();
            }

            var projection = await projectionRepository.RetrieveAsync<TestProjection>("TestDenormalizedProjection");

            projection.LastThing.Should().Be(aggregate.LastThing);
        }

    }
}