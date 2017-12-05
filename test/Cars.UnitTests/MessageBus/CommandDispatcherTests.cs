using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cars.Commands;
using Cars.MessageBus;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cars.UnitTests.MessageBus
{
    public class CommandDispatcherTests
    {
        private const string _categoryName = "Unit";
        private const string _categoryValue = "Command dispatcher";

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async void When_a_single_Command_is_published_to_the_bus_containing_a_single_CommandHandler()
        {
            var handler = new FirstTestCommandHandler();

	        var testCommand = new TestCommand(Guid.NewGuid());

			List<Action<TestCommand>> handlers = new List<Action<TestCommand>>
            {
                command => handler.ExecuteAsync(command)
            };

            Mock<ICommandDispatcher> commandDispatcherMock = new Mock<ICommandDispatcher>();
            commandDispatcherMock.Setup(e => e.DispatchAsync(It.IsAny<TestCommand>())).Callback((ICommand command) =>
            {
                handlers.ForEach(action =>
                {
                    action((TestCommand) command);
                });
            }).Returns(Task.FromResult(new DefaultResponse(testCommand.AggregateId)));


            ICommandDispatcher commandDispatcher = commandDispatcherMock.Object;

            await commandDispatcher.DispatchAsync(testCommand);

            handler.Ids.First().Should().Be(testCommand.AggregateId);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public async void When_a_single_Command_is_published_to_the_bus_containing_multiple_CommandHandlers()
        {
            var handler1 = new FirstTestCommandHandler();
            var handler2 = new SecondTestCommandHandler();

            List<Action<TestCommand>> handlers = new List<Action<TestCommand>>
            {
                command => handler1.ExecuteAsync(command),
                command => handler2.ExecuteAsync(command)
            };

            Mock<ICommandDispatcher> commandDispatcherMock = new Mock<ICommandDispatcher>();
            commandDispatcherMock.Setup(e => e.DispatchAsync(It.IsAny<TestCommand>())).Callback((ICommand command) =>
            {
                handlers.ForEach(action =>
                {
                    action((TestCommand)command);
                });
            }).Returns(Task.FromResult(new DefaultResponse(Guid.NewGuid())));

            var testCommand = new TestCommand(Guid.NewGuid());

            ICommandDispatcher commandDispatcher = commandDispatcherMock.Object;

            await commandDispatcher.DispatchAsync(testCommand);

            handler1.Ids.First().Should().Be(testCommand.AggregateId);
            handler2.Ids.First().Should().Be(testCommand.AggregateId);
        }

        public class FirstTestCommandHandler : ICommandHandler<TestCommand, DefaultResponse>
		{
            public List<Guid> Ids { get; } = new List<Guid>();

            public Task<DefaultResponse> ExecuteAsync(TestCommand command)
            {
                Ids.Add(command.AggregateId);

                return Task.FromResult(new DefaultResponse(command.AggregateId));
            }
        }

        public class SecondTestCommandHandler : ICommandHandler<TestCommand, DefaultResponse>
        {
            public List<Guid> Ids { get; } = new List<Guid>();

            public Task<DefaultResponse> ExecuteAsync(TestCommand command)
            {
                Ids.Add(command.AggregateId);

                return Task.FromResult(new DefaultResponse(command.AggregateId));
            }
        }

        public class TestCommand : CommandBase
        {
            public TestCommand(Guid id) : base(id)
            {
            }
        }
    }
}