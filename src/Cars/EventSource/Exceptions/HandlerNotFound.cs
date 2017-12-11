using System;

namespace Cars.EventSource.Exceptions
{
    public class HandlerNotFound : Exception
    {
        public Type EventType { get; }

        public HandlerNotFound(Type eventType)
        {
            EventType = eventType;
        }
    }
}