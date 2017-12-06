using System;
using System.Threading.Tasks;
using Cars.Demo.Command.Services.Carts.Commands.CreateCart;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Demo.Api.Query.Controllers
{
    [Route("query/carts")]
    public class CartReadController : Controller
    {
        private readonly ICreateCartHandler _createCartHandler;

        public CartReadController(ICreateCartHandler createCartHandler)
        {
            _createCartHandler = createCartHandler;
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> CreateCart([FromRoute]Guid cartId)
        {
            //var response = await _createCartHandler.ExecuteAsync(new CreateCartCommand(request.UserId));
            return Ok();
        }

    }
}
