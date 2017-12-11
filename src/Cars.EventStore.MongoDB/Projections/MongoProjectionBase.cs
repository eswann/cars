using Cars.Projections;
using MongoDB.Bson.Serialization.Attributes;

namespace Cars.EventStore.MongoDB.Projections
{
    public abstract class MongoProjectionBase : ProjectionBase
    {

        [BsonId]
        public override string ProjectionId { get; protected set; }

    }
}
