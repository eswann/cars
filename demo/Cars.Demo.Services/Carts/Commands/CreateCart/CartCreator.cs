using System;
using Cars.Demo.Domain.Events;
using Cars.EventSource;

namespace Cars.Demo.Services.Carts.Commands.CreateCart
{
    public class CartCreator : Aggregate
    {
        public CartCreator(string userId)
        {
            Emit(new CartCreated(Guid.NewGuid(), userId));
        }

        protected override void RegisterEvents()
        {
            SubscribeTo<CartCreated>(OnCartCreated);
        }

        private void OnCartCreated(CartCreated evt)
        {
            AggregateId = evt.AggregateId;
        }
    }
}