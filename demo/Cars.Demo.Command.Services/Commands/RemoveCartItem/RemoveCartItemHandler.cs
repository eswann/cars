using System.Threading.Tasks;
using Cars.Commands;
using Cars.Demo.Command.Services.Domain;
using Cars.EventSource;
using Cars.EventSource.Storage;

namespace Cars.Demo.Command.Services.Commands.RemoveCartItem
{
    public class RemoveCartItemHandler : IRemoveCartItemHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository _repository;

        public RemoveCartItemHandler(IUnitOfWork unitOfWork, IRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<DefaultResponse> ExecuteAsync(RemoveCartItemCommand command)
        {
            var cart = await _repository.GetByIdAsync<Cart>(command.CartId);
            cart.RemoveCartItem(command.Sku);
            
	        await _repository.AddAsync(cart);
	        await _unitOfWork.CommitAsync();

            return new DefaultResponse(cart.AggregateId);
	    }

    }
}
