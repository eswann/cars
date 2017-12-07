using System;
using System.Collections.Generic;
using System.Linq;
using Cars.Demo.Events;
using Cars.EventSource;

namespace Cars.Demo.Command.Services.Domain
{
    public class Cart : Aggregate
    {
        private readonly List<CartItem> _cartItems = new List<CartItem>();

        public Cart() { }

        public Cart(string userId)
        {
            Emit(new CartCreated(Guid.NewGuid(), userId));
        }

        public void AddCartItem(string sku, string name, decimal price, int quantity, bool customerTopRated, string image)
        {
            var existingCartItem = _cartItems.FirstOrDefault(x => x.Sku == sku);
            if (existingCartItem != null)
            {
                UpdatedCartItemQuantity(sku, existingCartItem.Quantity + quantity);
            }
            else
            {
                Emit(new CartItemAdded(AggregateId, sku, name, price, quantity, customerTopRated, image));
            }
        }

        public void UpdatedCartItemQuantity(string sku, int quantity)
        {
            AssertCartItemExists(sku);
            if (quantity == 0)
            {
                RemoveCartItem(sku);
            }
            else
            {
                Emit(new CartItemQuantityUpdated(AggregateId, sku, quantity));
            }
        }

        public void RemoveCartItem(string sku)
        {
            AssertCartItemExists(sku);
            Emit(new CartItemRemoved(AggregateId, sku));
        }

        private void AssertCartItemExists(string sku)
        {
            if (_cartItems.All(x => x.Sku != sku))
            {
                throw new InvalidOperationException("Cart item was not found in the cart");
            }
        }

        /// <summary>
        /// Here's where we register event handlers
        /// </summary>
        protected override void RegisterEvents()
        {
            SubscribeTo<CartCreated>(OnCartCreated);
            SubscribeTo<CartItemAdded>(OnCartItemAdded);
            SubscribeTo<CartItemQuantityUpdated>(OnCartQuantityUpdated);
            SubscribeTo<CartItemRemoved>(OnCartItemRemoved);
        }

        // We take actions internally on an event here

        private void OnCartCreated(CartCreated evt)
        {
            AggregateId = evt.AggregateId;
        }

        private void OnCartItemAdded(CartItemAdded evt)
        {
            _cartItems.Add(new CartItem(evt.Sku, evt.Name, evt.SalePrice, evt.Quantity));
        }

        private void OnCartQuantityUpdated(CartItemQuantityUpdated evt)
        {
            var cartItem = _cartItems.First(x => x.Sku == evt.Sku);
            cartItem.UpdateQuantity(evt.Quantity);
        }

        private void OnCartItemRemoved(CartItemRemoved evt)
        {
            var cartItem = _cartItems.First(x => x.Sku == evt.Sku);
            _cartItems.Remove(cartItem);
        }
    }
}