using System;

namespace Cars.Commands
{
    public class CommandBase : ICommand
    {
        public CommandBase(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
    }
}