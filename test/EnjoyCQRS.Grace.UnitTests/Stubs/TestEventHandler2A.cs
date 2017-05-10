using System.Threading.Tasks;
using EnjoyCQRS.Events;

namespace EnjoyCQRS.Grace.UnitTests.Stubs
{
    public class TestEventHandler2A : IEventHandler<TestEvent2>
    {
        public Task ExecuteAsync(TestEvent2 @event)
        {
            @event.WasHandled = true;
            return Task.CompletedTask;
        }
    }
}