using System.Threading.Tasks;
using Cars.Commands;
using Cars.Demo.Command.Services.Domain;
using Cars.EventSource;
using Cars.EventSource.Storage;

namespace Cars.Demo.Command.Services.Commands.RemoveCartItem
{
    public class RemoveCartItemHandler : IRemoveCartItemHandler
    {
        private readonly ISession _session;
        private readonly IRepository _repository;

        public RemoveCartItemHandler(ISession unitOfWork, IRepository repository)
        {
            _session = unitOfWork;
            _repository = repository;
        }

        public async Task<DefaultResponse> ExecuteAsync(RemoveCartItemCommand command)
        {
            var cart = await _repository.GetByIdAsync<Cart>(command.CartId);
            cart.RemoveCartItem(command.Sku);
            
	        _repository.Add(cart);
	        await _session.CommitAsync();

            return new DefaultResponse(cart.AggregateId);
	    }

    }
}
