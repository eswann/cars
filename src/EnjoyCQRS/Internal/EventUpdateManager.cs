using System;
using System.Collections.Generic;
using System.Reflection;
using EnjoyCQRS.Core;
using EnjoyCQRS.Events;
using Microsoft.Extensions.DependencyInjection;

namespace EnjoyCQRS.Internal
{
    internal class EventUpdateManager : IEventUpdateManager
    {
        private readonly Dictionary<Type, object> _eventUpdate = new Dictionary<Type, object>();
        private readonly IServiceProvider _serviceProvider;

        public EventUpdateManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IDomainEvent> Update(IEnumerable<IDomainEvent> events)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var eventUpdateTypeOf = typeof(IEventUpdate<>);

                var updatedEvents = new List<IDomainEvent>();

                foreach (var @event in events)
                {
                    var eventType = @event.GetType();

                    object eventUpdate;

                    if (!_eventUpdate.ContainsKey(eventType))
                    {
                        eventUpdate = scope.ServiceProvider.GetService(eventUpdateTypeOf.MakeGenericType(eventType));

                        if (eventUpdate == null)
                        {
                            updatedEvents.Add(@event);

                            continue;
                        }

                        _eventUpdate.Add(eventType, eventUpdate);
                    }
                    
                    eventUpdate = _eventUpdate[eventType];

                    var methodInfo = eventUpdate.GetType().GetMethod(nameof(IEventUpdate<object>.Update), BindingFlags.Instance | BindingFlags.Public);
                    var eventUpdateResult = (IEnumerable<IDomainEvent>)methodInfo.Invoke(eventUpdate, new object[] { @event });
                    
                    updatedEvents.AddRange(eventUpdateResult);
                }

                return updatedEvents;
            }
        }
    }
}
