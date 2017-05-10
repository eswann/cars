using System.Threading.Tasks;
using EnjoyCQRS.Commands;

namespace EnjoyCQRS.Grace.UnitTests.Stubs
{
    public class TestCommandHandler2 : ICommandHandler<TestCommand2>
    {
        public Task ExecuteAsync(TestCommand2 command)
        {
            command.WasHandled = true;

            return Task.CompletedTask;
        }
    }
}