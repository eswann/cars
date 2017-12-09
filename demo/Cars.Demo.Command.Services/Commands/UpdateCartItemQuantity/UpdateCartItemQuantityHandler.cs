using System.Threading.Tasks;
using Cars.Commands;
using Cars.Demo.Command.Services.Domain;
using Cars.EventSource;
using Cars.EventSource.Storage;

namespace Cars.Demo.Command.Services.Commands.UpdateCartItemQuantity
{
    public class UpdateCartItemQuantityHandler : IUpdateCartItemQuantityHandler
    {
        private readonly ISession _session;
        private readonly IRepository _repository;

        public UpdateCartItemQuantityHandler(ISession unitOfWork, IRepository repository)
        {
            _session = unitOfWork;
            _repository = repository;
        }

        public async Task<DefaultResponse> ExecuteAsync(UpdateCartItemQuantityCommand command)
        {
            var cart = await _repository.GetByIdAsync<Cart>(command.CartId);
            cart.UpdatedCartItemQuantity(command.Sku, command.Quantity);
            
	        _repository.Add(cart);
	        await _session.CommitAsync();

            return new DefaultResponse(cart.AggregateId);
	    }

    }
}
