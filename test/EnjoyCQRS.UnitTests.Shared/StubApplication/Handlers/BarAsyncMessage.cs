using System.Threading.Tasks;
using EnjoyCQRS.Handlers;

namespace EnjoyCQRS.UnitTests.Shared.StubApplication.Handlers
{
    public class TransientBarAsyncMessage : IAsyncMessage
    {
        public int Counter { get; set; }
    }

    public class TransientBarAsyncMessageHandler : IAsyncMessageHandler<TransientBarAsyncMessage>
    {
        private int _counter;
        
        public Task ExecuteAsync(TransientBarAsyncMessage message)
        {
            _counter++;

            message.Counter = _counter;

            return Task.CompletedTask;
        }
    }

    public class ScopedBarAsyncMessage : IAsyncMessage
    {
        public int Counter { get; set; }
    }

    public class ScopedBarAsyncMessageHandler : IAsyncMessageHandler<ScopedBarAsyncMessage>
    {
        private int _counter;

        public Task ExecuteAsync(ScopedBarAsyncMessage message)
        {
            _counter++;

            message.Counter = _counter;

            return Task.CompletedTask;
        }
    }
}
