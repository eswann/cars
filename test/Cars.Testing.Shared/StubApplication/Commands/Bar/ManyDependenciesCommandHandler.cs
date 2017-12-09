using System;
using System.Threading.Tasks;
using Cars.Commands;
using Cars.EventSource.Storage;
using Cars.Testing.Shared.StubApplication.Domain;

namespace Cars.Testing.Shared.StubApplication.Commands.Bar
{
    public class ManyDependenciesCommandHandler : ICommandHandler<ManyDependenciesCommand, DefaultResponse>
    {
        private readonly IRepository _repository;
        private readonly IBooleanService _booleanService;
        private readonly IStringService _stringService;

        public string Output { get; private set; }

        public ManyDependenciesCommandHandler(IRepository repository, IBooleanService booleanService, IStringService stringService)
        {
            _repository = repository;
            _booleanService = booleanService;
            _stringService = stringService;
        }

        public Task<DefaultResponse> ExecuteAsync(ManyDependenciesCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Text))
                throw new ArgumentNullException(nameof(command.Text));

            if (_booleanService.DoSomething())
            {
                Output = _stringService.PrintWithFormat(command.Text);
            }

            _repository.Add(Domain.Bar.Bar.Create(Guid.NewGuid()));

	        return Task.FromResult(new DefaultResponse(command.AggregateId));
        }
    }
}