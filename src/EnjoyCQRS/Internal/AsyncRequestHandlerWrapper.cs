using EnjoyCQRS.Handlers;
using System.Threading.Tasks;

namespace EnjoyCQRS.Internal
{
    internal abstract class AsyncRequestHandlerWrapper<TResult>
    {
        public abstract Task<TResult> ExecuteAsync(IAsyncRequest<TResult> message);
    }

    internal class AsyncRequestHandlerWrapper<TRequest, TResult> : AsyncRequestHandlerWrapper<TResult>
        where TRequest : IAsyncRequest<TResult>
    {
        private readonly IAsyncRequestHandler<TRequest, TResult> _inner;

        public AsyncRequestHandlerWrapper(IAsyncRequestHandler<TRequest, TResult> inner)
        {
            _inner = inner;
        }

        public override Task<TResult> ExecuteAsync(IAsyncRequest<TResult> request)
        {
            return _inner.ExecuteAsync((TRequest)request);
        }
    }
}