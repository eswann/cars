using System.Threading.Tasks;
using EnjoyCQRS.Commands;
using EnjoyCQRS.MessageBus;
using Grace.DependencyInjection;

namespace EnjoyCQRS.Grace
{
    public class GraceCommandDispatcher : CommandDispatcher
    {
        private readonly IExportLocatorScope _scope;

        public GraceCommandDispatcher(IExportLocatorScope scope)
        {
            _scope = scope;
        }

        protected override async Task RouteAsync<TCommand>(TCommand command)
        {
            var handler = _scope.Locate<ICommandHandler<TCommand>>();
            await handler.ExecuteAsync(command);
        }
    }
}
