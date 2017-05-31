using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Cars.Core;
using Cars.EventSource.Projections;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Cars.EventStore.MongoDB
{
    public class MongoProjectionRepository : IProjectionRepository, IDisposable
    {
	    private readonly ITextSerializer _bsonTextSerializer = new BsonTextSerializer();

        public MongoProjectionRepository(IMongoClient client, IMongoEventStoreSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Client = client ?? throw new ArgumentNullException(nameof(client));

            Settings.Validate();
        }

	    protected IMongoEventStoreSettings Settings { get; }

	    protected IMongoClient Client { get; }

		public async Task<object> GetAsync(Type projectionType, string category, Guid id)
        {
            var builderFilter = Builders<MongoProjection>.Filter;
            var filter = builderFilter.Eq(x => x.Category, category)
                         & builderFilter.Eq(x => x.ProjectionId, id);

            var projection = await QuerySingleResult(projectionType, filter);

            return projection;
        }

        public async Task<object> GetAsync(Type projectionType, string name)
        {
            var builderFilter = Builders<MongoProjection>.Filter;
            var filter = builderFilter.Eq(x => x.Id, name);

            var projection = await QuerySingleResult(projectionType, filter);

            return projection;
        }

        public async Task<TProjection> GetAsync<TProjection>(string name)
        {
            return (TProjection)(await GetAsync(typeof(TProjection), name));
        }
        
        public async Task<TProjection> GetAsync<TProjection>(string category, Guid id)
        {
            return (TProjection)(await GetAsync(typeof(TProjection), category, id));
        }

        private async Task<object> QuerySingleResult(Type projectionType, FilterDefinition<MongoProjection> filter)
        {
            var db = Client.GetDatabase(Settings.Database);
            var collection = db.GetCollection<MongoProjection>(Settings.ProjectionsCollectionName);
            
            var record = await collection
                .Find(filter)
                .Limit(1)
                .FirstAsync();

            var projection = _bsonTextSerializer.Deserialize(record.Projection.ToJson(), projectionType.AssemblyQualifiedName);
            
            return projection;
        }
        
        public void Dispose()
        {
        }

    }

	public class MongoProjectionRepository<TProjection> : MongoProjectionRepository, IProjectionRepository<TProjection>
	{
		public MongoProjectionRepository(MongoClient client, IMongoEventStoreSettings settings) : base(client, settings)
		{
		}

		public async Task<TProjection> GetAsync(string name)
		{
			return (TProjection)await GetAsync(typeof(TProjection), name);
		}

		public async Task<TProjection> GetAsync(Guid id)
		{
			var category = ExtractCategoryOfType<TProjection>();

			return (TProjection)await GetAsync(typeof(TProjection), category, id);
		}

		public Task<IEnumerable<TProjection>> FindAsync(Expression<Func<TProjection, bool>> expr)
		{
			var category = ExtractCategoryOfType<TProjection>();

			return FindAsync(category, expr);
		}

		public async Task<IEnumerable<TProjection>> FindAsync(string category, Expression<Func<TProjection, bool>> expr)
		{
			var db = Client.GetDatabase(Settings.Database);
			var collection = db.GetCollection<MongoProjection>(Settings.ProjectionsCollectionName).AsQueryable();

			var query = collection.Where(e => e.Category == category).Select(e => e.Projection).OfType<TProjection>().Where(expr);

			return await query.ToListAsync();
		}

		private string ExtractCategoryOfType<T>()
		{
			var type = typeof(T);

			var category = type.Name;

			if (char.IsUpper(type.Name[0]) && type.Name.StartsWith("I") && type.GetTypeInfo().IsInterface)
				category = typeof(TProjection).Name.Substring(1);

			return category;
		}
	}
}
