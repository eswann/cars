using System;
using Cars.Events;

namespace Cars.Demo.Domain.Events
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

        public string Sku { get; }
        public string Name { get; }
        public decimal Price { get; }
        public int Quantity { get; }

    }
}