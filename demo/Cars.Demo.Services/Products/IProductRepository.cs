using System.Collections.Generic;
using System.Threading.Tasks;
using Cars.Demo.Models;

namespace Cars.Demo.Services.Products
{
    public interface IProductRepository
    {
        Task<IList<Product>> GetProductListAsync();
    }
}