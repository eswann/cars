using System.Threading.Tasks;
using Cars.Commands;
using Cars.EventSource.Storage;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
    public class CreateFooCommandHandler : ICommandHandler<CreateFooCommand>
    {
        private readonly IRepository _repository;

        public CreateFooCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(CreateFooCommand command)
        {
            var foo = new Domain.Foo.Foo(command.StreamId);
            await _repository.AddAsync(foo);
        }
    }
}