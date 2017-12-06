using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cars.Demo.Query.Services.Products
{
    public interface IProductRepository
    {
        Task<IList<Product>> GetProductListAsync();
    }
}