using System.Threading.Tasks;
using Cars.Projections;
using MongoDB.Driver;

namespace Cars.EventStore.MongoDB
{
    public class MongoProjectionRepository : IProjectionRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoProjectionRepository(IMongoClient mongoClient, IMongoEventStoreSettings mongoEventStoreSettings)
        {
            _mongoDatabase = mongoClient.GetDatabase(mongoEventStoreSettings.Database);
        }

        public Task InsertAsync<TProjection>(TProjection projection) where TProjection : IProjection
        {
            var collection = GetCollection<TProjection>();
            collection.InsertOneAsync(projection);

            return Task.CompletedTask;
        }

        public Task UpdateAsync<TProjection>(TProjection projection) where TProjection: IProjection
        {
            var collection = GetCollection<TProjection>();
            collection.ReplaceOneAsync(x => x.ProjectionId == projection.ProjectionId, projection);

            return Task.CompletedTask;
        }

        public Task UpsertAsync<TProjection>(TProjection projection) where TProjection : IProjection
        {
            var collection = GetCollection<TProjection>();
            collection.ReplaceOneAsync(x => x.ProjectionId == projection.ProjectionId, projection, new UpdateOptions{IsUpsert = true});

            return Task.CompletedTask;
        }

        public async Task<TProjection> RetrieveAsync<TProjection>(object projectionId) where TProjection : IProjection
        {
            var collection = GetCollection<TProjection>();
            return (await collection.FindAsync(x => x.ProjectionId == projectionId.ToString())).FirstOrDefault();
        }

        private IMongoCollection<TProjection> GetCollection<TProjection>() where TProjection : IProjection
        {
            var collection = _mongoDatabase.GetCollection<TProjection>(typeof(TProjection).Name)
                .WithWriteConcern(WriteConcern.Acknowledged);
            return collection;
        }
    }

}