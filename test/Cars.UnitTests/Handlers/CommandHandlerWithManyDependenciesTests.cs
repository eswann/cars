using System;
using Cars.Commands;
using Cars.Testing.Shared.MessageBus;
using Cars.Testing.Shared.StubApplication.Commands.Bar;
using Cars.Testing.Shared.StubApplication.Domain;
using Cars.Testing.Shared.StubApplication.Domain.Bar;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cars.UnitTests.Handlers
{
    public class PrintSomethingTests : CommandTestFixture<ManyDependenciesCommand, DefaultResponse, ManyDependenciesCommandHandler, Bar>
    {
        protected override void SetupDependencies()
        {
            OnDependency<IBooleanService>().Setup(e => e.DoSomething()).Returns(true);
            OnDependency<IStringService>()
                .Setup(e => e.PrintWithFormat(It.IsAny<string>()))
                .Returns<string>(str => $"** {str} **");
        }

        protected override ManyDependenciesCommand When()
        {
            return new ManyDependenciesCommand("Hello World");
        }

        [Fact]
        public void Should_output_formatted_text()
        {
            CommandHandler.Output.Should().Be("** Hello World **");
        }
    }

    public class CaughtExceptionCommandHandlerTests : CommandTestFixture<ManyDependenciesCommand, DefaultResponse, ManyDependenciesCommandHandler, Bar>
    {
        protected override void SetupDependencies()
        {
            OnDependency<IBooleanService>().Setup(e => e.DoSomething()).Returns(true);
            OnDependency<IStringService>().Setup(e => e.PrintWithFormat(It.IsAny<string>()));
        }

        protected override ManyDependenciesCommand When()
        {
            return new ManyDependenciesCommand(string.Empty);
        }

        [Fact]
        public void Should_throw_ArgumentNullException()
        {
            CaughtException.Should().BeOfType<ArgumentNullException>();
        }
    }
}