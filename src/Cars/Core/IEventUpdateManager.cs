using System.Collections.Generic;
using Cars.Events;

namespace Cars.Core
{
    public interface IEventUpdateManager
    {
        IEnumerable<IDomainEvent> Update(IEnumerable<IDomainEvent> events);
    }
}