using System.Threading.Tasks;

namespace Cars.Demo.Query.Services.RemovedProducts
{
    public interface IRemovedProductsRepository
    {
        Task<RemovedProductsProjection> GetProjectionAsync();
    }
}