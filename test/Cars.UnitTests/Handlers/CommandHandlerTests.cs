using System;
using System.Threading.Tasks;
using Cars.Commands;
using Cars.Testing.Shared.MessageBus;
using Cars.UnitTests.Domain.Stubs;
using FluentAssertions;
using Xunit;

namespace Cars.UnitTests.Handlers
{
    public class CommandHandlerTests : CommandTestFixture<CommandHandlerTests.CreateStubCommand, CommandHandlerTests.CreateStubCommandHandler, StubStream>
    {
        private const string CategoryName = "Unit";
        private const string CategoryValue = "Handlers";

        private Guid _id;

        protected override CreateStubCommand When()
        {
            _id = Guid.NewGuid();

            return new CreateStubCommand(_id);
        }
        
        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Executed_property_should_be_true()
        {
            CommandHandler.Executed.Should().Be(true);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_pass_the_correct_StreamId()
        {
            CommandHandler.StreamId.Should().Be(_id);
        }

        public class CreateStubCommand : Command
        {
            public CreateStubCommand(Guid streamId) : base(streamId)
            {
            }
        }

        public class CreateStubCommandHandler : ICommandHandler<CreateStubCommand>
        {
            public bool Executed { get; set; }
            public Guid StreamId { get; set; }

            public Task ExecuteAsync(CreateStubCommand command)
            {
                Executed = true;
                StreamId = command.StreamId;

                return Task.CompletedTask;
            }
        }
    }
}