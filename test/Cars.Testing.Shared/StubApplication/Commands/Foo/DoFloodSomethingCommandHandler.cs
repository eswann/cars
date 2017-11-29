using System.Threading.Tasks;
using Cars.Commands;
using Cars.EventSource.Storage;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
    public class DoFloodSomethingCommandHandler : ICommandHandler<DoFloodSomethingCommand, DefaultResponse>
    {
        private readonly IRepository _repository;

        public DoFloodSomethingCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<DefaultResponse> ExecuteAsync(DoFloodSomethingCommand command)
        {
            var foo = await _repository.GetByIdAsync<Domain.Foo.Foo>(command.AggregateId);

            for (var i = 1; i <= command.Times; i++)
            {
                foo.DoSomething();
            }
			return new DefaultResponse(command.AggregateId);
        }
    }
}