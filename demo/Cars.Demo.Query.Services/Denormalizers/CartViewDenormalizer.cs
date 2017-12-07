using System.Linq;
using System.Threading.Tasks;
using Cars.Demo.Events;
using Cars.Demo.Query.Services.Carts;
using Cars.Events;
using Cars.Projections;
using Mapster;

namespace Cars.Demo.Query.Services.Denormalizers
{
    public class CartViewDenormalizer : 
        IEventHandler<CartCreated>,
        IEventHandler<CartItemAdded>,
        IEventHandler<CartItemQuantityUpdated>,
        IEventHandler<CartItemRemoved>
    {
        private readonly IProjectionRepository _projectionRepository;

        public CartViewDenormalizer(IProjectionRepository projectionRepository)
        {
            _projectionRepository = projectionRepository;
        }

        public async Task ExecuteAsync(CartCreated evt)
        {
            var cartView = new CartView
            {
                CartId = evt.AggregateId,
                UserId = evt.UserId
            };

            await _projectionRepository.InsertAsync(cartView);
        }

        public async Task ExecuteAsync(CartItemAdded evt)
        {
            var cartView = await _projectionRepository.RetrieveAsync<CartView>(evt.AggregateId.ToString());
            cartView.Products.Add(evt.Adapt<CartView.CartProduct>());
            await _projectionRepository.UpdateAsync(cartView);
        }

        public async Task ExecuteAsync(CartItemQuantityUpdated evt)
        {
            var cartView = await _projectionRepository.RetrieveAsync<CartView>(evt.AggregateId.ToString());
            var cartItem = cartView.Products.First(x => x.Sku == evt.Sku);
            cartItem.Quantity = evt.Quantity;
            await _projectionRepository.UpdateAsync(cartView);
        }

        public async Task ExecuteAsync(CartItemRemoved evt)
        {
            var cartView = await _projectionRepository.RetrieveAsync<CartView>(evt.AggregateId.ToString());
            var cartItem = cartView.Products.First(x => x.Sku == evt.Sku);
            cartView.Products.Remove(cartItem);
            await _projectionRepository.UpdateAsync(cartView);
        }
    }
}
