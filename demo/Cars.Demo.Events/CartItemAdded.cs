using System;
using Cars.Events;

namespace Cars.Demo.Events
{
    public class CartItemAdded : DomainEvent
    {
        public CartItemAdded() { }

        public CartItemAdded(Guid aggregateId, string sku, string name, decimal price, int quantity) : base(aggregateId)
        {
            Sku = sku;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public string Sku { get; protected set; }
        public string Name { get; protected set; }
        public decimal Price { get; protected set; }
        public int Quantity { get; protected set; }

    }
}