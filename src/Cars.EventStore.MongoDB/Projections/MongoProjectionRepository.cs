using System.Threading.Tasks;
using Cars.Events;
using Cars.Projections;
using MongoDB.Driver;

namespace Cars.EventStore.MongoDB.Projections
{
    public class MongoProjectionRepository : IProjectionRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoProjectionRepository(IMongoClient mongoClient, IMongoEventStoreSettings mongoEventStoreSettings)
        {
            _mongoDatabase = mongoClient.GetDatabase(mongoEventStoreSettings.Database);
        }

        public async Task UpsertAsync<TProjection>(TProjection projection, IDomainEvent lastEvent) where TProjection : IProjection
        {
            ((ProjectionMetadata) projection.Metadata).Timestamp = lastEvent.Metadata.Timestamp;
            var collection = GetCollection<TProjection>();
            await collection.ReplaceOneAsync(x => x.ProjectionId == projection.ProjectionId, projection, new UpdateOptions { IsUpsert = true });
        }

        public async Task<TProjection> RetrieveAsync<TProjection>(object projectionId) where TProjection : IProjection
        {
            var collection = GetCollection<TProjection>();
            return (await collection.FindAsync(x => x.ProjectionId == projectionId.ToString())).FirstOrDefault();
        }

        public async Task DropProjectionAsync<TProjection>() where TProjection : IProjection
        {
            await _mongoDatabase.DropCollectionAsync(typeof(TProjection).Name);
        }

        private IMongoCollection<TProjection> GetCollection<TProjection>() where TProjection : IProjection
        {
            var collection = _mongoDatabase.GetCollection<TProjection>(typeof(TProjection).Name);
            return collection;
        }
    }

}