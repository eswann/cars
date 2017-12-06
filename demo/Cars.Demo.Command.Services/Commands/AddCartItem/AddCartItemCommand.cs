using System;

namespace Cars.Demo.Command.Services.Carts.Commands.AddCartItem
{
    public class AddCartItemCommand
    {
        public AddCartItemCommand(Guid cartId, string sku, string name, decimal price, int quantity)
        {
            CartId = cartId;
            Sku = sku;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public Guid CartId { get; }
        public string Sku { get; }
        public string Name { get; }
        public decimal Price { get; }
        public int Quantity { get; }
    }
}
