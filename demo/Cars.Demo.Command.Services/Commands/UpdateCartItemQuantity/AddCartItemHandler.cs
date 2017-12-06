using System.Threading.Tasks;
using Cars.Commands;
using Cars.Demo.Command.Services.Domain;
using Cars.EventSource;
using Cars.EventSource.Storage;

namespace Cars.Demo.Command.Services.Carts.Commands.UpdateCartItemQuantity
{
    public class UpdateCartItemQuantityHandler : IUpdateCartItemQuantityHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository _repository;

        public UpdateCartItemQuantityHandler(IUnitOfWork unitOfWork, IRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<DefaultResponse> ExecuteAsync(UpdateCartItemQuantityCommand command)
        {
            var cart = await _repository.GetByIdAsync<Cart>(command.CartId);
            cart.UpdatedCartItemQuantity(command.Sku, command.Quantity);
            
	        await _repository.AddAsync(cart);
	        await _unitOfWork.CommitAsync();

            return new DefaultResponse(cart.AggregateId);
	    }

    }
}
