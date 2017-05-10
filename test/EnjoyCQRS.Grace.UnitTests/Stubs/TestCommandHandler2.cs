using System.Threading.Tasks;
using EnjoyCQRS.Commands;

namespace EnjoyCQRS.Grace.UnitTests.Stubs
{
    public class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public Task ExecuteAsync(TestCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}