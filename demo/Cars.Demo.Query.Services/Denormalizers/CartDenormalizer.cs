using System.Linq;
using System.Threading.Tasks;
using Cars.Demo.Events;
using Cars.Demo.Query.Services.Projections;
using Cars.Events;
using Cars.Projections;
using Mapster;

namespace Cars.Demo.Query.Services.Denormalizers
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
            var cartView = new CartView
            {
                CartId = evt.AggregateId,
                UserId = evt.UserId
            };

            await _projectionRepository.Insert(cartView);
        }

        public async Task ExecuteAsync(CartItemAdded evt)
        {
            var cartView = await _projectionRepository.Retrieve<CartView>(evt.AggregateId.ToString());
            cartView.Products.Add(evt.Adapt<CartView.CartProduct>());
            await _projectionRepository.Update(cartView);
        }

        public async Task ExecuteAsync(CartItemQuantityUpdated evt)
        {
            var cartView = await _projectionRepository.Retrieve<CartView>(evt.AggregateId.ToString());
            var cartItem = cartView.Products.First(x => x.Sku == evt.Sku);
            cartItem.Quantity = evt.Quantity;
            await _projectionRepository.Update(cartView);
        }

        public async Task ExecuteAsync(CartItemRemoved evt)
        {
            var cartView = await _projectionRepository.Retrieve<CartView>(evt.AggregateId.ToString());
            var cartItem = cartView.Products.First(x => x.Sku == evt.Sku);
            cartView.Products.Remove(cartItem);
            await _projectionRepository.Update(cartView);
        }
    }
}
