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

namespace Cars.EventSource.Storage
{
    public class AggregateTracker
    {
        private readonly ConcurrentDictionary<Type, Dictionary<Guid, object>> _track = new ConcurrentDictionary<Type, Dictionary<Guid, object>>();

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : IAggregate
        {
            if (!_track.TryGetValue(typeof(TAggregate), out var aggregates))
                return default(TAggregate);

            if (!aggregates.TryGetValue(id, out var aggregate))
                return default(TAggregate);

            return (TAggregate)aggregate;
        }
        
        public void Add<TAggregate>(TAggregate projection) where TAggregate : IAggregate
        {
            if (!_track.TryGetValue(typeof(TAggregate), out var aggregates))
            {
                aggregates = new Dictionary<Guid, object>();
                _track.TryAdd(typeof(TAggregate), aggregates);
            }

            if (aggregates.ContainsKey(projection.AggregateId))
                return;

            aggregates.Add(projection.AggregateId, projection);
        }

        public void Remove(Type aggregateType, Guid aggregateId)
        {
            if (!_track.TryGetValue(aggregateType, out var aggregates))
            {
                return;
            }

            aggregates.Remove(aggregateId);
        }
        
    }
}