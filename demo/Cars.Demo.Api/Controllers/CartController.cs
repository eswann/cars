using System.Threading.Tasks;
using Cars.Demo.Api.Models;
using Cars.Demo.Services.Carts.Commands.CreateCart;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Demo.Api.Controllers
{
    [Route("carts")]
    public class CartController : Controller
    {
        private readonly ICreateCartHandler _createCartHandler;

        public CartController(ICreateCartHandler createCartHandler)
        {
            _createCartHandler = createCartHandler;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody]CreateCartRequest request)
        {
            var response = await _createCartHandler.ExecuteAsync(new CreateCartCommand(request.UserId));

            return Ok(response);
        }
        
    }
}
