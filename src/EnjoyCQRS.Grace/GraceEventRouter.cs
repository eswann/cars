using System.Threading.Tasks;
using EnjoyCQRS.Events;
using EnjoyCQRS.MessageBus;
using Grace.DependencyInjection;

namespace EnjoyCQRS.Grace
{
    public class GraceEventRouter : IEventRouter
    {
        private readonly IInjectionScope _scope;

        public GraceEventRouter(IInjectionScope scope)
        {
            _scope = scope;
        }

        public async Task RouteAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
        {
            var handlers = _scope.LocateAll<IEventHandler<TEvent>>();

            foreach (var handler in handlers)
            {
                await handler.ExecuteAsync(@event).ConfigureAwait(false);
            }
        }
    }
}
