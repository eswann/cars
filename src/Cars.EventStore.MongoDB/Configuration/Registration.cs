using System;
using Cars.Configuration;
using Cars.Core;
using Cars.EventSource.Projections;
using Cars.EventSource.Storage;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Cars.EventStore.MongoDB.Configuration
{
    public static class Registration
    {
        public static IServiceCollection AddCarsMongo(this IServiceCollection services)
        {
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("camelCase", pack, t => true);
            BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

            services.AddCars();

            services.AddSingleton<ITextSerializer, BsonTextSerializer>();
            services.AddSingleton<IMongoClient>(provider => new MongoClient(provider.GetService<IMongoEventStoreSettings>().ConnectionString));
            services.AddSingleton<IEventStore, MongoEventStore>();
			services.AddSingleton<IProjectionRepository, MongoProjectionRepository>();
	        services.AddSingleton(typeof(IProjectionRepository<>), typeof(MongoProjectionRepository<>));

            return services;
		}

	}
}
