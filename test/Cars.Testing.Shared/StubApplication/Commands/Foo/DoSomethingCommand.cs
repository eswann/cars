using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
    public class DoSomethingCommand : Command
    {
        public DoSomethingCommand(Guid streamId) : base(streamId)
        {
        }
    }
}