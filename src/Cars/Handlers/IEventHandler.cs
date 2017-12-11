using System.Threading.Tasks;
using Cars.Events;

namespace Cars.Handlers
{
    public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task ExecuteAsync(TEvent @event);

    }
}