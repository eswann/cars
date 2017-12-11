using System;

namespace Cars.EventSource.Exceptions
{
    public class EventTypeNotFoundException : Exception
    {
        public string Type { get; }
        
        public EventTypeNotFoundException(string type) : base ($"Could not be found the type: ${type}.")
        {
            Type = type;
        }
    }
}