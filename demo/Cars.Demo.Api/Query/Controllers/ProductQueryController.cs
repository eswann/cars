using System.Threading.Tasks;
using Cars.Demo.Query.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Demo.Api.Query.Controllers
{
    [Route("query/products")]
    public class ProductQueryController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductQueryController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductListAsync()
        {
            return Ok(await _productRepository.GetProductListAsync());
        }
        
    }
}
