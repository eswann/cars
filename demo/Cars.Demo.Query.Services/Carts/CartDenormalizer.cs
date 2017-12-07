using System.Linq;
using System.Threading.Tasks;
using Cars.Demo.Events;
using Cars.Events;
using Cars.Projections;
using Mapster;

namespace Cars.Demo.Query.Services.Carts
{
    public class CartDenormalizer : 
        IEventHandler<CartCreated>,
        IEventHandler<CartItemAdded>,
        IEventHandler<CartItemQuantityUpdated>,
        IEventHandler<CartItemRemoved>
    {
        private readonly IProjectionRepository _projectionRepository;

        public CartDenormalizer(IProjectionRepository projectionRepository)
        {
            _projectionRepository = projectionRepository;
        }

        public async Task ExecuteAsync(CartCreated evt)
        {
            var cartProjection = new CartProjection
            {
                CartId = evt.AggregateId,
                UserId = evt.UserId
            };

            await _projectionRepository.InsertAsync(cartProjection);
        }

        public async Task ExecuteAsync(CartItemAdded evt)
        {
            var cartProjection = await _projectionRepository.RetrieveAsync<CartProjection>(evt.AggregateId.ToString());
            cartProjection.Products.Add(evt.Adapt<CartProjection.CartProduct>());
            await _projectionRepository.UpdateAsync(cartProjection);
        }

        public async Task ExecuteAsync(CartItemQuantityUpdated evt)
        {
            var cartProjection = await _projectionRepository.RetrieveAsync<CartProjection>(evt.AggregateId.ToString());
            var cartItem = cartProjection.Products.First(x => x.Sku == evt.Sku);
            cartItem.Quantity = evt.Quantity;
            await _projectionRepository.UpdateAsync(cartProjection);
        }

        public async Task ExecuteAsync(CartItemRemoved evt)
        {
            var cartProjection = await _projectionRepository.RetrieveAsync<CartProjection>(evt.AggregateId.ToString());
            var cartItem = cartProjection.Products.First(x => x.Sku == evt.Sku);
            cartProjection.Products.Remove(cartItem);
            await _projectionRepository.UpdateAsync(cartProjection);
        }
    }
}
