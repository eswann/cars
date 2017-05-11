using System;
using EnjoyCQRS.Commands;

namespace EnjoyCQRS.Testing.Shared.StubApplication.Commands.FooAggregate
{
    public class DoSomethingCommand : Command
    {
        public DoSomethingCommand(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}