using System.Linq;
using System.Threading.Tasks;
using Cars.Demo.Events;
using Cars.Events;
using Cars.Projections;

namespace Cars.Demo.Query.Services.RemovedProducts
{
    public class RemovedProductsDenormalizer : IEventHandler<CartItemRemoved>
    {
        private readonly IProjectionRepository _projectionRepository;

        public RemovedProductsDenormalizer(IProjectionRepository projectionRepository)
        {
            _projectionRepository = projectionRepository;
        }

        public async Task ExecuteAsync(CartItemRemoved evt)
        {
            var removedProductsProjection = await _projectionRepository.RetrieveAsync<RemovedProductsProjection>(Constants.ProjectionId) 
                ?? new RemovedProductsProjection();
            var product = removedProductsProjection.Products.FirstOrDefault(x => x.Sku == evt.Sku);
            if (product == null)
            {
                product = new RemovedProductsProjection.RemovedProductTotal {Sku = evt.Sku, Count = 1};
                removedProductsProjection.Products.Add(product);
            }
            else
            {
                product.Count++;
            }
            await _projectionRepository.UpsertAsync(removedProductsProjection);
        }
    }
}
