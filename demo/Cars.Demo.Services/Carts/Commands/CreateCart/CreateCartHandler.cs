using System.Threading.Tasks;
using Cars.Commands;

namespace Cars.Demo.Services.Commands.CreateCart
{
    public class CreateCartHandler : ICommandHandler<CreateCartCommand, CreateCartResponse>
    {
	    public Task<CreateCartResponse> ExecuteAsync(CreateCartCommand command)
	    {
		    return Task.FromResult(new CreateCartResponse(command.AggregateId));
	    }
    }
}
