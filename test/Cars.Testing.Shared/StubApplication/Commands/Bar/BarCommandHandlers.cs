using System.Threading.Tasks;
using Cars.Commands;
using Cars.EventSource.Storage;

namespace Cars.Testing.Shared.StubApplication.Commands.Bar
{
    public class BarCommandHandlers : ICommandHandler<CreateBarCommand, CreateBarResponse>, ICommandHandler<SpeakCommand, DefaultResponse>
    {
        private readonly IRepository _repository;

        public BarCommandHandlers(IRepository repository)
        {
            _repository = repository;
        }

        public Task<CreateBarResponse> ExecuteAsync(CreateBarCommand command)
        {
            var bar = Domain.Bar.Bar.Create(command.AggregateId);

            _repository.Add(bar);

	        return Task.FromResult(new CreateBarResponse(bar.AggregateId));
        }

        public async Task<DefaultResponse> ExecuteAsync(SpeakCommand command)
        {
            var bar = await _repository.GetByIdAsync<Domain.Bar.Bar>(command.AggregateId).ConfigureAwait(false);

            bar.Speak(command.Text);

			return new DefaultResponse(command.AggregateId);
        }
    }
}