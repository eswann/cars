using System.Threading.Tasks;
using Cars.Demo.Query.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Demo.Api.Query.Controllers
{
    [Route("query/products")]
    public class ProductReadController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductReadController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductList()
        {
            return Ok(await _productRepository.GetProductListAsync());
        }
        
    }
}
