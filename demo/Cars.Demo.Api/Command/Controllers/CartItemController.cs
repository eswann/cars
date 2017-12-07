using System;
using System.Threading.Tasks;
using Cars.Demo.Api.Command.Models;
using Cars.Demo.Command.Services.Commands.AddCartItem;
using Cars.Demo.Command.Services.Commands.RemoveCartItem;
using Cars.Demo.Command.Services.Commands.UpdateCartItemQuantity;
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

        [HttpPut("{sku}")]
        public async Task<IActionResult> AddCartItemAsync([FromRoute]Guid cartId, [FromRoute]string sku, [FromBody]AddCartItemRequest request)
        {
            await _addCartItemHandler.ExecuteAsync(
                new AddCartItemCommand(cartId, sku, request.Name, 
                request.SalePrice, request.Quantity, 
                request.CustomerTopRated, request.Image));
            return Ok();
        }

        [HttpPut("{sku}/quantity")]
        public async Task<IActionResult> UpdateCartItemQuantityAsync([FromRoute]Guid cartId, [FromRoute]string sku, [FromBody]UpdateCartQuantityRequest request)
        {
            await _updateItemQuantityHandler.ExecuteAsync(new UpdateCartItemQuantityCommand(cartId, sku, request.Quantity));
            return Ok();
        }

        [HttpDelete("{sku}")]
        public async Task<IActionResult> RemoveCartItemAsync([FromRoute]Guid cartId, [FromRoute]string sku)
        {
            await _removeCartItemHandler.ExecuteAsync(new RemoveCartItemCommand(cartId, sku));
            return NoContent();
        }

    }
}
