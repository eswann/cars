using System.Threading.Tasks;
using Cars.Demo.Events;
using Cars.Demo.Query.Services.Projections;
using Cars.Events;
using Cars.EventStore.MongoDB;
using MongoDB.Driver;

namespace Cars.Demo.Query.Services.Denormalizers
{
    public class CartDenormalizer : IEventHandler<CartCreated>
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoEventStoreSettings _mongoEventStoreSettings; 

        public CartDenormalizer(IMongoClient mongoClient, IMongoEventStoreSettings mongoEventStoreSettings)
        {
            _mongoClient = mongoClient;
            _mongoEventStoreSettings = mongoEventStoreSettings;
        }

        public async Task ExecuteAsync(CartCreated @event)
        {
            var cartView = new CartView
            {
                CartId = @event.AggregateId,
                UserId = @event.UserId
            };
            

            var database = _mongoClient.GetDatabase(_mongoEventStoreSettings.Database);
            var collection = database.GetCollection<CartView>(typeof(CartView).Name);

            await collection.InsertOneAsync(cartView);
        }
    }
}
