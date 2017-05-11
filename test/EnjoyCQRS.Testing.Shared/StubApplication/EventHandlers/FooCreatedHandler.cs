using System.Threading.Tasks;
using EnjoyCQRS.Events;
using EnjoyCQRS.Testing.Shared.StubApplication.Domain.FooAggregate;

namespace EnjoyCQRS.Testing.Shared.StubApplication.EventHandlers
{
    public class FooCreatedHandler : IEventHandler<FooCreated>
    {
        public Task ExecuteAsync(FooCreated @event)
        {
            return Task.CompletedTask;
        }
    }
}
