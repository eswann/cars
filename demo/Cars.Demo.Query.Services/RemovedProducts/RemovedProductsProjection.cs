using System.Collections.Generic;
using Cars.EventStore.MongoDB.Projections;

namespace Cars.Demo.Query.Services.RemovedProducts
{
    public class RemovedProductsProjection : MongoProjectionBase
    {
        public override string ProjectionId { get; protected set; } = Constants.ProjectionId;

        public IList<RemovedProductTotal> Products = new List<RemovedProductTotal>();

        public class RemovedProductTotal
        {
            public string Sku { get; set; }

            public int Count { get; set; }
        }
    }

}
