using System;

namespace Cars.Demo.Command.Services.Commands.AddCartItem
{
    public class AddCartItemCommand
    {
        public AddCartItemCommand(Guid cartId, string sku, string name, decimal salePrice, int quantity, bool customerTopRated, string image)
        {
            CartId = cartId;
            Sku = sku;
            Name = name;
            SalePrice = salePrice;
            Quantity = quantity;
            CustomerTopRated = customerTopRated;
            Image = image;
        }

        public Guid CartId { get; }
        public string Sku { get; }
        public string Name { get; }
        public decimal SalePrice { get; }
        public int Quantity { get; }
        public bool CustomerTopRated { get; }
        public string Image { get; }
    }
}
