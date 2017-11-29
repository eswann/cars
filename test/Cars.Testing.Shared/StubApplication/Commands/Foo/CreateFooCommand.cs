using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
    public class CreateFooCommand : Command
    {
        public CreateFooCommand(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}