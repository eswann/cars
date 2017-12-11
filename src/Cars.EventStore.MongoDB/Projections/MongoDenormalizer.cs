using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Cars.Core;
using Cars.EventSource.SerializedEvents;
using Cars.EventSource.Storage;
using Cars.Handlers;
using Cars.Projections;

namespace Cars.EventStore.MongoDB.Projections
{
    public abstract class MongoDenormalizer : IDenormalizer
    {
        private static readonly ConcurrentDictionary<Type, List<Type>> _denormalizerEventTypes = new ConcurrentDictionary<Type, List<Type>>();
        private static readonly ConcurrentDictionary<Type, Dictionary<Type, Func<object, object, Task>>> _denormalizerEventHandlers = 
            new ConcurrentDictionary<Type, Dictionary<Type, Func<object, object, Task>>>();

        private readonly IEventStore _eventStore;
        private readonly IEventSerializer _eventSerializer;

        protected IProjectionRepository Repository;


        protected MongoDenormalizer(IProjectionRepository repository, IEventStore eventStore, IEventSerializer eventSerializer)
        {
            Repository = repository;
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;

            var denormalizerType = GetType();

            if (!_denormalizerEventTypes.ContainsKey(denormalizerType))
            {
                var eventTypes = GetType().GetTypeInfo().GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    .SelectMany(i => i.GetGenericArguments())
                    .ToList();
                _denormalizerEventTypes.TryAdd(denormalizerType, eventTypes);

                var eventHandlerMethods = denormalizerType.GetTypeInfo()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.ReturnType == typeof(Task) && x.GetParameters().Length == 1 &&
                                eventTypes.Contains(x.GetParameters()[0].ParameterType));

                var eventHandlerDelegates = new Dictionary<Type, Func<object, object, Task>>();
                foreach (var method in eventHandlerMethods)
                {
                    var eventType = method.GetParameters()[0].ParameterType;
                    var instanceParam = Expression.Parameter(typeof(object), "instance");
                    var eventParam = Expression.Parameter(typeof(object), "event");
                    var handlerDelegate = Expression.Lambda<Func<object, object, Task>>(
                            Expression.Call(Expression.Convert(instanceParam, denormalizerType), method, Expression.Convert(eventParam, eventType)), instanceParam, eventParam)
                        .Compile();
                    eventHandlerDelegates.Add(eventType, handlerDelegate);
                }
                _denormalizerEventHandlers.TryAdd(denormalizerType, eventHandlerDelegates);
            }
        }

        public bool IsOffline { get; set; }

        public async Task RebuildAsync<TProjection>(DateTime? fromTimestamp = null) where TProjection : IProjection
        {
            IsOffline = true;
            await Repository.DropProjectionAsync<TProjection>();

            if (_denormalizerEventTypes.TryGetValue(GetType(), out var eventTypesToReplay) && eventTypesToReplay.Count > 0)
            {
                if (_denormalizerEventHandlers.TryGetValue(GetType(), out var eventHandlers))
                {
                    var committedEvents = await _eventStore.GetEventsByTypeAsync(eventTypesToReplay, fromTimestamp.GetValueOrDefault());

                    if (committedEvents.Count > 0)
                    {
                        var events = committedEvents.Select(_eventSerializer.Deserialize);
                        foreach (var evt in events)
                        {
                            var handler = eventHandlers[evt.GetType()];
                            await handler.Invoke(this, evt);
                        }
                    }
                }
            }

            IsOffline = false;

        }
    }
}