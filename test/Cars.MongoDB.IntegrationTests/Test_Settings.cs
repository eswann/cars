using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Cars.MongoDB.IntegrationTests
{
    public static class Test_Settings
    {
	    public const string MongoHost = "localhost:27017";

        static Test_Settings()
        {
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register("camelCase", pack, t => true);
            BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        }
    }
}
