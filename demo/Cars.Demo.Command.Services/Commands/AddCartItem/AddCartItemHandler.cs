using System.Threading.Tasks;
using Cars.Commands;
using Cars.Demo.Command.Services.Domain;
using Cars.EventSource.Storage;

namespace Cars.Demo.Command.Services.Commands.AddCartItem
{
    public class CreateCartHandler : IAddCartItemHandler
    {
        private readonly ISession _session;
        private readonly IRepository _repository;

        public CreateCartHandler(ISession session, IRepository repository)
        {
            _session = session;
            _repository = repository;
        }

        public async Task<DefaultResponse> ExecuteAsync(AddCartItemCommand command)
        {
            var cart = await _repository.GetByIdAsync<Cart>(command.CartId);
            cart.AddCartItem(command.Sku, command.Name, command.SalePrice, command.Quantity, command.CustomerTopRated, command.Image);

            _repository.Add(cart);
            await _session.CommitAsync();

            return new DefaultResponse(cart.AggregateId);
	    }

    }
}
