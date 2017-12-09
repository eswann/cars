// The MIT License (MIT)
// 
// Copyright (c) 2016 Nelson Corrêa V. Júnior
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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