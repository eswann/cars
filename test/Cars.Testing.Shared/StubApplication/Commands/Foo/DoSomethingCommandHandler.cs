using System.Threading.Tasks;
using Cars.Commands;
using Cars.EventSource.Storage;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
    public class DoSomethingCommandHandler : ICommandHandler<DoSomethingCommand, DoSomethingResponse>
    {
        private readonly IRepository _repository;

        public DoSomethingCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<DoSomethingResponse> ExecuteAsync(DoSomethingCommand command)
        {
            var foo = await _repository.GetByIdAsync<Domain.Foo.Foo>(command.AggregateId);
            foo.DoSomething();
	        return new DoSomethingResponse(command.AggregateId);
        }
    }
}