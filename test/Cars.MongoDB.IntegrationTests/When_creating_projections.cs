using System;
using System.Threading.Tasks;
using Cars.EventStore.MongoDB;
using Cars.Projections;
using FluentAssertions;
using MongoDB.Bson.Serialization.Attributes;
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
            if (string.IsNullOrWhiteSpace(Test_Settings.MongoHost)) throw new NullReferenceException("The variable 'MONGODB_HOST' was not configured.");

            _mongoClient = new MongoClient($"mongodb://{Test_Settings.MongoHost}");

            _mongoClient.DropDatabase(_defaultSettings.Database);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Projection_can_be_stored_and_retrieved()
        {
            var projectionRepo = new MongoProjectionRepository(_mongoClient, _defaultSettings);

            var projectionId = Guid.NewGuid().ToString();
            var firstName = "FirstName" + projectionId;
            await projectionRepo.InsertAsync(new TestProjection{ProjectionId = projectionId, FirstName = firstName});

            await Task.Delay(200);

            var projectionResult = await projectionRepo.RetrieveAsync<TestProjection>(projectionId);

            projectionResult.ProjectionId.Should().Be(projectionId);
            projectionResult.FirstName.Should().Be(firstName);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task Projection_can_be_updated()
        {
            var projectionRepo = new MongoProjectionRepository(_mongoClient, _defaultSettings);

            var projectionId = Guid.NewGuid().ToString();
            var firstName = "FirstName" + projectionId;
            await projectionRepo.InsertAsync(new TestProjection { ProjectionId = projectionId, FirstName = firstName });

            await Task.Delay(200);

            var projectionResult = await projectionRepo.RetrieveAsync<TestProjection>(projectionId);
            projectionResult.LastName = "UpdatedLastName";

            await projectionRepo.UpdateAsync(projectionResult);

            await Task.Delay(200);

            projectionResult = await projectionRepo.RetrieveAsync<TestProjection>(projectionId);
            projectionResult.LastName.Should().Be("UpdatedLastName");
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async Task New_projection_can_be_upserted()
        {
            var projectionRepo = new MongoProjectionRepository(_mongoClient, _defaultSettings);

            var projectionId = Guid.NewGuid().ToString();
            var firstName = "FirstName" + projectionId;
            await projectionRepo.UpsertAsync(new TestProjection { ProjectionId = projectionId, FirstName = firstName });

            await Task.Delay(200);

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
            await projectionRepo.InsertAsync(new TestProjection { ProjectionId = projectionId, FirstName = firstName });

            await Task.Delay(200);

            var projectionResult = await projectionRepo.RetrieveAsync<TestProjection>(projectionId);
            projectionResult.LastName = "UpdatedLastName";

            await projectionRepo.UpsertAsync(projectionResult);

            await Task.Delay(200);

            projectionResult = await projectionRepo.RetrieveAsync<TestProjection>(projectionId);
            projectionResult.LastName.Should().Be("UpdatedLastName");
        }

        class TestProjection : IProjection
        {
            [BsonId]
            public string ProjectionId { get; set; }

            public string FirstName { get; set; } = "FirstName";

            public string LastName { get; set; } = "LastName";

        }
    }
}
