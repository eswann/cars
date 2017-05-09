using System.Threading.Tasks;
using EnjoyCQRS.Commands;
using EnjoyCQRS.MessageBus;
using Grace.DependencyInjection;

namespace EnjoyCQRS.Grace
{
    public class GraceCommandDispatcher : CommandDispatcher
    {
        private readonly IInjectionScope _scope;

        public GraceCommandDispatcher(IInjectionScope scope)
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
