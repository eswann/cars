using System.Threading.Tasks;
using Cars.Demo.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Demo.Api.Controllers
{
    [Route("products")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
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
