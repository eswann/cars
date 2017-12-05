using System.Threading.Tasks;
using Cars.Demo.Api.Models;
using Cars.Demo.Services.Carts.Commands.CreateCart;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Demo.Api.Controllers
{
    [Route("carts")]
    public class CartController : Controller
    {        
        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody]CreateCartRequest request)
        {
	        var command = request.Adapt<CreateCartCommand>();

            return Ok();
        }
        
    }
}
