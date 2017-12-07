using System.Threading.Tasks;
using Cars.Commands;
using Cars.Demo.Command.Services.Domain;
using Cars.EventSource;
using Cars.EventSource.Storage;

namespace Cars.Demo.Command.Services.Commands.CreateCart
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

        public async Task<DefaultResponse> ExecuteAsync(CreateCartCommand command)
	    {   
            var cartCreator = new Cart(command.UserId);
	        await _repository.AddAsync(cartCreator);
	        await _unitOfWork.CommitAsync();

            return new DefaultResponse(cartCreator.AggregateId);
	    }

    }
}
