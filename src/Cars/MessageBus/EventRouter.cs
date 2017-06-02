using System.Threading.Tasks;
using Cars.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.MessageBus
{
    public class EventRouter : IEventRouter
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventRouter(IServiceScopeFactory scopeFactory)
        {
	        _scopeFactory = scopeFactory; 
        }

        public async Task RouteAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
        {
	        using (var scope = _scopeFactory.CreateScope())
	        {
		        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();
		        foreach (var handler in handlers)
		        {
			        await handler.ExecuteAsync(@event).ConfigureAwait(false);
		        }
	        }
        }
    }
}
