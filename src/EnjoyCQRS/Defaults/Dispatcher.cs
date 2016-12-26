// The MIT License (MIT)
// 
// Copyright (c) 2016 Nelson Corrêa V. Júnior
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using EnjoyCQRS.Handlers;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using EnjoyCQRS.Internal;
using System.Linq;
using System.Collections.Concurrent;

namespace EnjoyCQRS.Defaults
{
    public class Dispatcher : IDispatcher
    {
        private readonly ConcurrentDictionary<Type, Type> _genericHandlerCache = new ConcurrentDictionary<Type, Type>();
        private readonly ConcurrentDictionary<Type, Type> _wrapperHandlerCache = new ConcurrentDictionary<Type, Type>();
        private readonly IServiceProvider _serviceProvider;

        public Dispatcher(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public Task PublishAsync<TMessage>(TMessage message) where TMessage : IAsyncMessage
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var handlers = GetHandlers(message).Select(h => h.ExecuteAsync(message));

            return Task.WhenAll(handlers);
        }
        
        public Task<TResponse> SendAsync<TResponse>(IAsyncRequest<TResponse> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var handler = GetHandler(request);

            return handler.ExecuteAsync(request);
        }

        private IEnumerable<AsyncMessageHandlerWrapper> GetHandlers(IAsyncMessage message)
        {
            return BuildHandlers<AsyncMessageHandlerWrapper>(message,
                typeof(IAsyncMessageHandler<>),
                typeof(AsyncMessageHandlerWrapper<>));
        }

        private IEnumerable<TWrapper> BuildHandlers<TWrapper>(object message, Type handlerType, Type wrapperType)
        {
            var messageType = message.GetType();

            var genericHandlerType = _genericHandlerCache.GetOrAdd(messageType, (m) => handlerType.MakeGenericType(m));
            var genericWrapperType = _wrapperHandlerCache.GetOrAdd(messageType, (m) => wrapperType.MakeGenericType(m));
            
            var handlers = _serviceProvider.GetServices(genericHandlerType) ?? Enumerable.Empty<object>();

            if (!handlers.Any()) throw new Exception($"Handler was not found for requested type: {messageType}.");

            return handlers
                .Select(handler => Activator.CreateInstance(genericWrapperType, handler))
                .Cast<TWrapper>();
        }

        private AsyncRequestHandlerWrapper<TResponse> GetHandler<TResponse>(IAsyncRequest<TResponse> request)
        {
            return BuildHandler<AsyncRequestHandlerWrapper<TResponse>, TResponse>(request,
                typeof(IAsyncRequestHandler<,>),
                typeof(AsyncRequestHandlerWrapper<,>));
        }

        private TWrapper BuildHandler<TWrapper, TResponse>(object request, Type handlerType, Type wrapperType)
        {
            var requestType = request.GetType();

            var genericHandlerType = _genericHandlerCache.GetOrAdd(requestType, (r) => handlerType.MakeGenericType(r, typeof(TResponse)));
            var genericWrapperType = _wrapperHandlerCache.GetOrAdd(requestType, (r) => wrapperType.MakeGenericType(r, typeof(TResponse)));
            
            var handler = _serviceProvider.GetService(genericHandlerType);

            if (handler == null) throw new Exception($"Handler was not found for requested type: {requestType}.");

            return (TWrapper)Activator.CreateInstance(genericWrapperType, handler);
        }

        public void Dispose()
        {
        }
    }
}