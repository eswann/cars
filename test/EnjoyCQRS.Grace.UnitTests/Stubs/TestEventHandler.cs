using System.Threading.Tasks;
using EnjoyCQRS.Events;

namespace EnjoyCQRS.Grace.UnitTests.Stubs
{
    public class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task ExecuteAsync(TestEvent @event)
        {
            @event.WasHandled = true;
            return Task.CompletedTask;
        }
    }
}