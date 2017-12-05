using System.Threading.Tasks;
using Cars.EventSource.Storage;

namespace Cars.Demo.Services.Carts.Commands.CreateCart
{
    public class CreateCartHandler : ICreateCartHandler
    {
        private readonly IRepository _repository;

        public CreateCartHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<CreateCartResponse> ExecuteAsync(CreateCartCommand command)
	    {   
            var cartCreator = new CartCreator(command.UserId);
	        _repository.AddAsync(cartCreator);

            return Task.FromResult(new CreateCartResponse(cartCreator.AggregateId));
	    }

    }
}
