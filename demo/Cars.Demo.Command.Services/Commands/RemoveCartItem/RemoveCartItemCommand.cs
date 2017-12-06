using System;

namespace Cars.Demo.Command.Services.Carts.Commands.RemoveCartItem
{
    public class RemoveCartItemCommand
    {
        public RemoveCartItemCommand(Guid cartId, string sku)
        {
            CartId = cartId;
            Sku = sku;
        }

        public Guid CartId { get; }
        public string Sku { get; }
    }
}
