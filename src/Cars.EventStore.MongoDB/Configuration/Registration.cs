using Cars.Configuration;
using Cars.Core;
using Cars.EventSource.Projections;
using Cars.EventSource.Storage;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Cars.EventStore.MongoDB.Configuration
{
    public static class Registration
    {
        public static IServiceCollection AddCarsMongo(this IServiceCollection services)
        {
	        services.AddCars();

            services.AddSingleton<IMongoClient>(provider => new MongoClient(provider.GetService<IMongoEventStoreSettings>().ConnectionString));
            services.AddSingleton<ITextSerializer, BsonTextSerializer>();
            services.AddSingleton<IEventStore, MongoEventStore>();
			services.AddSingleton<IProjectionRepository, MongoProjectionRepository>();
	        services.AddSingleton(typeof(IProjectionRepository<>), typeof(MongoProjectionRepository<>));

			return services;
		}

	}
}
