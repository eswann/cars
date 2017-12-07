using System;
using Cars.Events;

namespace Cars.Demo.Events
{
    public class CartItemAdded : DomainEvent
    {
        public CartItemAdded() { }

        public CartItemAdded(Guid aggregateId, string sku, string name, decimal salePrice,
            int quantity, bool customerTopRated, string image) : base(aggregateId)
        {
            Sku = sku;
            Name = name;
            SalePrice = salePrice;
            Quantity = quantity;
            CustomerTopRated = customerTopRated;
            Image = image;
        }

        public string Sku { get; protected set; }
        public string Name { get; protected set; }
        public decimal SalePrice { get; protected set; }
        public int Quantity { get; protected set; }
        public bool CustomerTopRated { get; protected set; }
        public string Image { get; protected set; }

    }
}