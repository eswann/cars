using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
    public class CreateFooCommand : CommandBase
    {
        public CreateFooCommand(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}