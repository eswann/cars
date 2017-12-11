using System.Threading.Tasks;
using Cars.Events;
using Cars.Handlers;
using Cars.Testing.Shared.StubApplication.Domain.Foo;

namespace Cars.Testing.Shared.StubApplication.EventHandlers
{
    public class FooCreatedHandler : IEventHandler<FooCreated>
    {
        public Task ExecuteAsync(FooCreated @event)
        {
            return Task.CompletedTask;
        }
    }
}
