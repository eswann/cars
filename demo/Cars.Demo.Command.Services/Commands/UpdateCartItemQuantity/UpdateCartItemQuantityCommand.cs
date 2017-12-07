using System;

namespace Cars.Demo.Command.Services.Commands.UpdateCartItemQuantity
{
    public class UpdateCartItemQuantityCommand
    {
        public UpdateCartItemQuantityCommand(Guid cartId, string sku, int quantity)
        {
            CartId = cartId;
            Sku = sku;
            Quantity = quantity;
        }

        public Guid CartId { get; }
        public string Sku { get; }
        public int Quantity { get; }
    }
}
