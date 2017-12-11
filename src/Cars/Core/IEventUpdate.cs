using System.Collections.Generic;
using Cars.Events;

namespace Cars.Core
{
    public interface IEventUpdate<TOldEvent> where TOldEvent : class 
    {
        IEnumerable<IDomainEvent> Update(TOldEvent oldEvent);
    }
}