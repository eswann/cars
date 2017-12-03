using Cars.Demo.Api.Models;
using Cars.Demo.Services.Commands.CreateCart;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Demo.Api.Controllers
{
    [Route("carts")]
    public class CartController : Controller
    {        
        [HttpPost]
        public void StartTrip([FromBody]CreateCartRequest request)
        {
	        var command = request.Adapt<CreateCartCommand>();
        }
        
    }
}
