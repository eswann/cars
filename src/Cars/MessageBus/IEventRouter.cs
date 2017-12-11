using System.Threading.Tasks;
using Cars.Events;

namespace Cars.MessageBus
{
    public interface IEventRouter
    {
        Task RouteAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent;
    }
}