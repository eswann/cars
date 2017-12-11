using System;
using System.Collections.Generic;
using System.Linq;

namespace Cars.EventSource.Exceptions
{
    public class AggregateConcurrencyException : Exception
    {
        public IList<IGrouping<Guid, IAggregate>> Aggregates { get; }

        public AggregateConcurrencyException(IList<IGrouping<Guid, IAggregate>> aggregates, string message, Exception innerException = null) : base(message, innerException)
        {
            Aggregates = aggregates;
        }
    }
}