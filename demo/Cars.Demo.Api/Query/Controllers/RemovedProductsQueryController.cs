using System;
using System.Threading.Tasks;
using Cars.Demo.Query.Services.RemovedProducts;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Demo.Api.Query.Controllers
{
    [Route("query/products/removed")]
    public class RemovedProductsQueryController : Controller
    {
        private readonly IRemovedProductsRepository _removedProductsRepository;

        public RemovedProductsQueryController(IRemovedProductsRepository removedProductsRepository)
        {
            _removedProductsRepository = removedProductsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetRemovedProductsAsync([FromRoute]Guid cartId)
        {
            var projection = await _removedProductsRepository.GetProjectionAsync();
            return Ok(projection.Adapt<Models.RemovedProductsProjection>());
        }

    }
}
