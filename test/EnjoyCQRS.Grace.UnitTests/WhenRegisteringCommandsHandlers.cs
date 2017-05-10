using System.Linq;
using EnjoyCQRS.Commands;
using EnjoyCQRS.Grace.UnitTests.Stubs;
using FluentAssertions;
using Grace.DependencyInjection;
using Xunit;

namespace EnjoyCQRS.Grace.UnitTests
{
    public class WhenRegisteringCommandHandlers
    {

        [Fact]
        public void Handlers_in_assembly_are_registered()
        {
            var container = new DependencyInjectionContainer();
            container.RegisterCommandHandlersInAssembly<TestCommandHandler>();

            var testCommandHandlers = container.LocateAll<ICommandHandler<TestCommand>>();
            testCommandHandlers.Count.Should().Be(1);
            testCommandHandlers.First().Should().BeOfType<TestCommandHandler>();

            var testCommandHandlers2 = container.LocateAll<ICommandHandler<TestCommand2>>();
            testCommandHandlers2.Count.Should().Be(1);
            testCommandHandlers2.First().Should().BeOfType<TestCommandHandler2>();
        }

    
    }

}