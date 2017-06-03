using System.Threading.Tasks;
using Cars.Commands;
using Cars.EventSource.Storage;

namespace Cars.Testing.Shared.StubApplication.Commands.Bar
{
    public class BarCommandHandlers : ICommandHandler<CreateBarCommand>, ICommandHandler<SpeakCommand>
    {
        private readonly IRepository _repository;

        public BarCommandHandlers(IRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(CreateBarCommand command)
        {
            var bar = Domain.Bar.Bar.Create(command.StreamId);

            await _repository.AddAsync(bar).ConfigureAwait(false);
        }

        public async Task ExecuteAsync(SpeakCommand command)
        {
            var bar = await _repository.GetByIdAsync<Domain.Bar.Bar>(command.StreamId).ConfigureAwait(false);

            bar.Speak(command.Text);
        }
    }
}