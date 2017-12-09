using System.Threading.Tasks;
using Cars.Commands;
using Cars.Demo.Command.Services.Domain;
using Cars.EventSource.Storage;

namespace Cars.Demo.Command.Services.Commands.CreateCart
{
    public class CreateCartHandler : ICreateCartHandler
    {
        private readonly ISession _session;
        private readonly IRepository _repository;

        public CreateCartHandler(ISession unitOfWork, IRepository repository)
        {
            _session = unitOfWork;
            _repository = repository;
        }

        public async Task<DefaultResponse> ExecuteAsync(CreateCartCommand command)
	    {   
            var cartCreator = new Cart(command.UserId);
	        _repository.Add(cartCreator);
	        await _session.CommitAsync();

            return new DefaultResponse(cartCreator.AggregateId);
	    }

    }
}
