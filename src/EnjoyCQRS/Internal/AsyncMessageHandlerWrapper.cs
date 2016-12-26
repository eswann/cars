using EnjoyCQRS.Handlers;
using System.Threading.Tasks;

namespace EnjoyCQRS.Internal
{
    internal abstract class AsyncMessageHandlerWrapper
    {
        public abstract Task ExecuteAsync(IAsyncMessage message);
    }

    internal class AsyncMessageHandlerWrapper<TMessage> : AsyncMessageHandlerWrapper
        where TMessage : IAsyncMessage
    {
        private readonly IAsyncMessageHandler<TMessage> _inner;

        public AsyncMessageHandlerWrapper(IAsyncMessageHandler<TMessage> inner)
        {
            _inner = inner;
        }

        public override Task ExecuteAsync(IAsyncMessage message)
        {
            return _inner.ExecuteAsync((TMessage)message);
        }
    }
}