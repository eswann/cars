using Cars.Configuration;
using Cars.Core;
using Cars.EventSource.Projections;
using Cars.EventSource.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.EventStore.MongoDB.Configuration
{
    public static class Registration
    {
        public static IServiceCollection AddCarsMongo(this IServiceCollection serviceCollection)
        {
	        serviceCollection.AddCars();

            serviceCollection.AddSingleton<ITextSerializer, BsonTextSerializer>();
            serviceCollection.AddSingleton<IEventStore, MongoEventStore>();
			serviceCollection.AddSingleton<IProjectionRepository, MongoProjectionRepository>();
	        serviceCollection.AddSingleton(typeof(IProjectionRepository<>), typeof(MongoProjectionRepository<>));

			return serviceCollection;
		}

	}
}
