using System;
using EnjoyCQRS.Commands;

namespace EnjoyCQRS.Testing.Shared.StubApplication.Commands.FooAggregate
{
    public class CreateFooCommand : Command
    {
        public CreateFooCommand(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}