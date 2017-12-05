using System.Threading.Tasks;
using Cars.EventSource;
using Cars.EventSource.Storage;

namespace Cars.Demo.Services.Carts.Commands.CreateCart
{
    public class CreateCartHandler : ICreateCartHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository _repository;

        public CreateCartHandler(IUnitOfWork unitOfWork, IRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<CreateCartResponse> ExecuteAsync(CreateCartCommand command)
	    {   
            var cartCreator = new CartCreator(command.UserId);
	        await _repository.AddAsync(cartCreator);
	        await _unitOfWork.CommitAsync();

            return new CreateCartResponse(cartCreator.AggregateId);
	    }

    }
}
