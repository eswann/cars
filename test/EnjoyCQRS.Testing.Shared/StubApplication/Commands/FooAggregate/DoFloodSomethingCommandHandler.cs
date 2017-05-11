using System.Threading.Tasks;
using EnjoyCQRS.Commands;
using EnjoyCQRS.EventSource.Storage;
using EnjoyCQRS.Testing.Shared.StubApplication.Domain.FooAggregate;

namespace EnjoyCQRS.Testing.Shared.StubApplication.Commands.FooAggregate
{
    public class DoFloodSomethingCommandHandler : ICommandHandler<DoFloodSomethingCommand>
    {
        private readonly IRepository _repository;

        public DoFloodSomethingCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(DoFloodSomethingCommand command)
        {
            var foo = await _repository.GetByIdAsync<Foo>(command.AggregateId);

            for (var i = 1; i <= command.Times; i++)
            {
                foo.DoSomething();
            }
        }
    }
}