using System;
using Cars.Events;

namespace Cars.Demo.Domain.Events
{
    public class CartItemQuantityUpdated : DomainEvent
    {
        public CartItemQuantityUpdated() { }

        public CartItemQuantityUpdated(Guid aggregateId, string sku, int quantity) : base(aggregateId)
        {
            Sku = sku;
            Quantity = quantity;
        }

        public string Sku { get; }
        public int Quantity { get; }

    }
}