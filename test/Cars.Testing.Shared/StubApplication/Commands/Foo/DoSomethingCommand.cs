using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
    public class DoSomethingCommand : CommandBase
    {
        public DoSomethingCommand(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}