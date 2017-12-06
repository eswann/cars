using System;
using Cars.Events;

namespace Cars.Demo.Events
{
    public class CartCreated : DomainEvent
    {
        public CartCreated() { }

        public CartCreated(Guid aggregateId, string userId) : base(aggregateId)
        {
            UserId = userId;
        }

        public string UserId { get; protected set; }
    }
}
