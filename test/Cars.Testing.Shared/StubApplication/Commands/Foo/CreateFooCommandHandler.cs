using System.Threading.Tasks;
using Cars.Commands;
using Cars.EventSource.Storage;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
    public class CreateFooCommandHandler : ICommandHandler<CreateFooCommand, CreateFooResponse>
    {
        private readonly IRepository _repository;

        public CreateFooCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<CreateFooResponse> ExecuteAsync(CreateFooCommand command)
        {
            var foo = new Domain.Foo.Foo(command.AggregateId);
            _repository.Add(foo);

			return Task.FromResult(new CreateFooResponse(command.AggregateId));
        }
    }
}