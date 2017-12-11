using System;
using System.Threading.Tasks;
using Cars.Events;
using Cars.EventStore.MongoDB;
using Cars.EventStore.MongoDB.Projections;
using FluentAssertions;
using MongoDB.Driver;
using Xunit;

namespace Cars.MongoDB.IntegrationTests
{
    public class When_creating_projections
    {
        private const string _categoryName = "Integration";
        private const string _categoryValue = "MongoDB";

        private readonly IMongoEventStoreSettings _defaultSettings = new MongoEventStoreSettings();
        private readonly MongoClient _mongoClient;

        public When_creating_projections()
        {
            if (string.IsNullOrWhiteSpace(TestSettings.MongoHost)) throw new NullReferenceException("The variable 'MONGODB_HOST' was not configured.");

            _mongoClient = new MongoClient($"mongodb://{TestSettings.MongoHost}");
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Projection_can_be_stored_and_retrieved()
        {
            var projectionRepo = new MongoProjectionRepository(_mongoClient, _defaultSettings);

            var projectionId = Guid.NewGuid().ToString();
            var firstName = "FirstName" + projectionId;
            await projectionRepo.UpsertAsync(new TestProjection(projectionId) { FirstName = firstName }, new TestEvent());

            var projectionResult = await projectionRepo.RetrieveAsync<TestProjection>(projectionId);

            projectionResult.ProjectionId.Should().Be(projectionId);
            projectionResult.FirstName.Should().Be(firstName);
        }


        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Existing_projection_can_be_upserted()
        {
            var projectionRepo = new MongoProjectionRepository(_mongoClient, _defaultSettings);

            var projectionId = Guid.NewGuid().ToString();
            var firstName = "FirstName" + projectionId;
            await projectionRepo.UpsertAsync(new TestProjection(projectionId) { FirstName = firstName }, new TestEvent());

            var projectionResult = await projectionRepo.RetrieveAsync<TestProjection>(projectionId);
            projectionResult.LastName = "UpdatedLastName";

            await projectionRepo.UpsertAsync(projectionResult, new TestEvent());
            await Task.Delay(200);

            projectionResult = await projectionRepo.RetrieveAsync<TestProjection>(projectionId);
            projectionResult.LastName.Should().Be("UpdatedLastName");
        }

        public class TestProjection : MongoProjectionBase
        {
            public TestProjection() { }

            public TestProjection(string projectionId)
            {
                ProjectionId = projectionId;
            }

            public string FirstName { get; set; } = "FirstName";

            public string LastName { get; set; } = "LastName";

        }

        public class TestEvent : DomainEvent
        {
            public override DomainEventMetadata Metadata => new DomainEventMetadata(DateTime.MinValue, 1);
        }
    }
}
