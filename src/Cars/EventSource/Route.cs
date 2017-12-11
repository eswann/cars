using System;
using System.Collections.Generic;
using Cars.EventSource.Exceptions;

namespace Cars.EventSource
{
    public class Route<T> : Dictionary<Type, Action<T>>
    {
        public void Handle(T @event)
        {
            var eventType = @event.GetType();

            if (!ContainsKey(eventType)) throw new HandlerNotFound(eventType);

            this[eventType](@event);
        }
    }
}