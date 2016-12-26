using EnjoyCQRS.Defaults;
using EnjoyCQRS.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EnjoyCQRS.UnitTests.Handlers
{
    public class AsyncRequestHandlerTests
    {
        private readonly IServiceProvider _serviceProvider;

        public AsyncRequestHandlerTests()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDispatcher, Dispatcher>();
            serviceCollection.AddScoped<IAsyncRequestHandler<SimpleAsyncRequest, SimpleAsyncResponse>, FirstAsyncRequestHandler>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task Should_execute_single_async_request_handler()
        {
            // Act
            var request = new SimpleAsyncRequest("async message");
            var dispatcher = _serviceProvider.GetService<IDispatcher>();

            var response = await dispatcher.SendAsync(request);

            // Assert
            response.Message.Should().Contain("async message with response");
        }

        [Fact]
        public void Should_throws_exception_when_not_found_handler()
        {
            // Act
            var request = new SimpleAsyncRequestWithoutHandler();
            var dispatcher = _serviceProvider.GetService<IDispatcher>();

            // Assert
            Func<Task> act = async () => await dispatcher.SendAsync(request);

            act.ShouldThrowExactly<Exception>().WithMessage($"Handler was not found for requested type: {request.GetType()}.");
        }
    }

    public class FirstAsyncRequestHandler : IAsyncRequestHandler<SimpleAsyncRequest, SimpleAsyncResponse>
    {
        public Task<SimpleAsyncResponse> ExecuteAsync(SimpleAsyncRequest request)
        {
            var response = new SimpleAsyncResponse(request.Message + " with response");

            return Task.FromResult(response);
        }
    }

    public class SimpleAsyncRequest : IAsyncRequest<SimpleAsyncResponse>
    {
        public string Message { get; }

        public SimpleAsyncRequest(string message)
        {
            Message = message;
        }
    }

    public class SimpleAsyncResponse
    {
        public string Message { get; }

        public SimpleAsyncResponse(string message)
        {
            Message = message;
        }
    }

    public class SimpleAsyncRequestWithoutHandler : IAsyncRequest<SimpleAsyncResponse>
    {
    }
}