using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Cars.MongoDB.IntegrationTests
{
    public static class TestSettings
    {
        public const string MongoHost = "localhost:27017";


        static TestSettings()
        {
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register("camelCase", pack, t => true);
            if (BsonSerializer.LookupSerializer<DateTime>() == null)
            {
                BsonSerializer.RegisterSerializer(DateTimeSerializer.LocalInstance);
            }
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        }
    }
}
