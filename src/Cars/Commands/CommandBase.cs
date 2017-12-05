using System;

namespace Cars.Commands
{
    public class CommandBase
    {
        public CommandBase(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
    }
}