using System;
using System.Threading.Tasks;
using Cars.Demo.Api.Command.Models;
using Cars.Demo.Command.Services.Carts.Commands.AddCartItem;
using Cars.Demo.Command.Services.Carts.Commands.RemoveCartItem;
using Cars.Demo.Command.Services.Carts.Commands.UpdateCartItemQuantity;
using Microsoft.AspNetCore.Mvc;

namespace Cars.Demo.Api.Command.Controllers
{
    [Route("command/carts/{cartId}/items")]
    public class CartItemController : Controller
    {
        private readonly IAddCartItemHandler _addCartItemHandler;
        private readonly IUpdateCartItemQuantityHandler _updateItemQuantityHandler;
        private readonly IRemoveCartItemHandler _removeCartItemHandler;

        public CartItemController(IAddCartItemHandler addCartItemHandler,
            IUpdateCartItemQuantityHandler updateItemQuantityHandler, IRemoveCartItemHandler removeCartItemHandler)
        {
            _addCartItemHandler = addCartItemHandler;
            _updateItemQuantityHandler = updateItemQuantityHandler;
            _removeCartItemHandler = removeCartItemHandler;
        }

        [HttpPost("{sku}")]
        public async Task<IActionResult> AddCartItem([FromRoute]Guid cartId, [FromRoute]string sku, [FromBody]AddCartItemRequest request)
        {
            await _addCartItemHandler.ExecuteAsync(new AddCartItemCommand(cartId, sku, request.Name, request.Price, request.Quantity));
            return Ok();
        }

        [HttpPut("{sku}")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromRoute]Guid cartId, [FromRoute]string sku, [FromBody]UpdateCartQuantityRequest request)
        {
            await _updateItemQuantityHandler.ExecuteAsync(new UpdateCartItemQuantityCommand(cartId, sku, request.Quantity));
            return Ok();
        }

        [HttpDelete("{sku}")]
        public async Task<IActionResult> RemoveCartItem([FromRoute]Guid cartId, [FromRoute]string sku)
        {
            await _removeCartItemHandler.ExecuteAsync(new RemoveCartItemCommand(cartId, sku));
            return NoContent();
        }

    }
}
