using System.Collections.Generic;
using Cars.Events;

namespace Cars.Collections
{
    /// <summary>
    /// Represents collection of commited events.
    /// </summary>
    public class CommitedDomainEventCollection : HashSet<IDomainEvent>
    {
        public CommitedDomainEventCollection(IEnumerable<IDomainEvent> events) : base(events)
        {
        }
    }
}