using Cars.Core;
using Cars.EventSource.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.EventStore.MongoDB
{
    public static class Registration
    {
        public static IServiceCollection AddMongo(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITextSerializer, BsonTextSerializer>();
            serviceCollection.AddSingleton<IEventStore, MongoEventStore>();

            return serviceCollection;
        }

        public static IServiceCollection AddEventStore(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITextSerializer, BsonTextSerializer>();
            serviceCollection.AddSingleton<IEventStore, MongoEventStore>();

            return serviceCollection;
        }
    }
}
