using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cars.EventSource.Storage
{
    public class AggregateTracker
    {
        private readonly ConcurrentDictionary<Type, Dictionary<Guid, Aggregate>> _tracker = new ConcurrentDictionary<Type, Dictionary<Guid, Aggregate>>();

        public IEnumerable<Aggregate> Aggregates => _tracker.Values.SelectMany(x => x.Values);

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : Aggregate
        {
            if (!_tracker.TryGetValue(typeof(TAggregate), out var aggregates))
                return default(TAggregate);

            if (!aggregates.TryGetValue(id, out Aggregate aggregate))
                return default(TAggregate);

            return (TAggregate)aggregate;
        }

        public void Add<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            if (!_tracker.TryGetValue(aggregate.GetType(), out var aggregates))
            {
                aggregates = new Dictionary<Guid, Aggregate>();
                _tracker.TryAdd(typeof(TAggregate), aggregates);
            }

            if (aggregates.ContainsKey(aggregate.AggregateId))
                return;

            aggregates.Add(aggregate.AggregateId, aggregate);
        }

        public void Remove(Type aggregateRootType, Guid aggregateId)
        {
            if (!_tracker.TryGetValue(aggregateRootType, out var aggregates))
            {
                return;
            }

            aggregates.Remove(aggregateId);
        }

        public void Clear()
        {
            _tracker.Clear();
        }

    }
}