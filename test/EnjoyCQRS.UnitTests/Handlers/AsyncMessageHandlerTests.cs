using EnjoyCQRS.Defaults;
using EnjoyCQRS.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EnjoyCQRS.UnitTests.Handlers
{
    public class AsyncMessageHandlerTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly StringBuilder _output = new StringBuilder();

        public AsyncMessageHandlerTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDispatcher, Dispatcher>();
            serviceCollection.AddScoped<TextWriter>(c => new StringWriter(_output));

            serviceCollection.AddScoped<IAsyncMessageHandler<SimpleAsyncMessage>, FirstAsyncMessageHandler>();
            serviceCollection.AddScoped<IAsyncMessageHandler<SimpleAsyncMessage>, SecondAsyncMessageHandler>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task Should_execute_single_async_message_handler()
        {
            // Act
            var message = new SimpleAsyncMessage("async message");
            var dispatcher = _serviceProvider.GetService<IDispatcher>();

            await dispatcher.PublishAsync(message);
            
            // Assert
            var result = _output.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            result[0].Should().Contain("async message");
        }

        [Fact]
        public async Task Should_execute_multiples_async_message_handlers()
        {
            // Act
            var message = new SimpleAsyncMessage("async message");
            var dispatcher = _serviceProvider.GetService<IDispatcher>();

            await dispatcher.PublishAsync(message);

            // Assert
            var result = _output.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            result[0].Should().Contain($"async message from {nameof(FirstAsyncMessageHandler)}");
            result[1].Should().Contain($"async message from {nameof(SecondAsyncMessageHandler)}");
        }

        [Fact]
        public void Should_throws_exception_when_not_found_handler()
        {
            // Act
            var message = new SimpleAsyncMessageWithoutHandler();
            var dispatcher = _serviceProvider.GetService<IDispatcher>();

            // Assert
            Func<Task> act = async () => await dispatcher.PublishAsync(message);

            act.ShouldThrowExactly<Exception>().WithMessage($"Handler was not found for requested type: {message.GetType()}.");
        }
    }

    public class FirstAsyncMessageHandler : IAsyncMessageHandler<SimpleAsyncMessage>
    {
        private readonly TextWriter _textWriter;

        public FirstAsyncMessageHandler(TextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        public async Task ExecuteAsync(SimpleAsyncMessage message)
        {
            await _textWriter.WriteLineAsync($"{message.Message} from {nameof(FirstAsyncMessageHandler)}");
        }
    }

    public class SecondAsyncMessageHandler : IAsyncMessageHandler<SimpleAsyncMessage>
    {
        private readonly TextWriter _textWriter;

        public SecondAsyncMessageHandler(TextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        public async Task ExecuteAsync(SimpleAsyncMessage message)
        {
            await _textWriter.WriteLineAsync($"{message.Message} from {nameof(SecondAsyncMessageHandler)}");
        }
    }

    public class SimpleAsyncMessage : IAsyncMessage
    {
        public string Message { get; }

        public SimpleAsyncMessage(string message)
        {
            Message = message;
        }
    }

    public class SimpleAsyncMessageWithoutHandler : IAsyncMessage
    {
    }
}