using System.Threading.Tasks;
using Cars.Commands;
using Cars.Demo.Domain.Events;
using Cars.EventSource;

namespace Cars.Demo.Services.Carts.Commands.CreateCart
{
    public class CreateCartHandler : Aggregate, ICommandHandler<CreateCartCommand, CreateCartResponse>
    {
	    public Task<CreateCartResponse> ExecuteAsync(CreateCartCommand command)
	    {
	        AggregateId = command.AggregateId;
	        Emit(new CartCreated(command.AggregateId, command.UserId));
            
            return Task.FromResult(new CreateCartResponse(command.AggregateId));
	    }

    }
}
