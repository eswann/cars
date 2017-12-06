using System;
using Cars.Events;

namespace Cars.Demo.Events
{
    public class CartItemQuantityUpdated : DomainEvent
    {
        public CartItemQuantityUpdated() { }

        public CartItemQuantityUpdated(Guid aggregateId, string sku, int quantity) : base(aggregateId)
        {
            Sku = sku;
            Quantity = quantity;
        }

        public string Sku { get; protected set; }
        public int Quantity { get; protected set; }

    }
}