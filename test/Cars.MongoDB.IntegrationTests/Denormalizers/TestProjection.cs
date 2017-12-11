using Cars.EventStore.MongoDB.Projections;

namespace Cars.MongoDB.IntegrationTests.Denormalizers
{
    public class TestProjection : MongoProjectionBase
    {

        public TestProjection()
        {
            ProjectionId = "TestDenormalizedProjection";
        }

        public string LastThing { get; set; }

    }
}
