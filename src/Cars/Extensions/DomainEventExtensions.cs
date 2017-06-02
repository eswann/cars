using System.Reflection;
using Cars.Attributes;
using Cars.Events;

namespace Cars.Extensions
{
    public static class DomainEventExtensions
    {
        public static bool TryGetEventNameAttribute(this IDomainEvent @event, out string eventName)
        {
	        var attribute = @event.GetType().GetTypeInfo().GetCustomAttribute<EventNameAttribute>();
            eventName = attribute?.EventName;
            
            return !string.IsNullOrWhiteSpace(eventName);
        }
    }
}