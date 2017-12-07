using System;
using System.Threading.Tasks;
using Cars.Demo.Query.Services.Carts;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Demo.Api.Query.Controllers
{
    [Route("query/carts")]
    public class CartQueryController : Controller
    {
        private readonly ICartRepository _cartRepository;

        public CartQueryController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartAsync([FromRoute]Guid cartId)
        {
            var projection = await _cartRepository.GetCartProjectionAsync(cartId);
            return Ok(projection.Adapt<Models.CartView>());
        }

    }
}
