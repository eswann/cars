﻿// The MIT License (MIT)
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cars.Events;
using Cars.EventSource.SerializedEvents;

namespace Cars.EventSource.Storage
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<ICommitedEvent> _events = new List<ICommitedEvent>();
        private readonly List<ISerializedEvent> _uncommitedEvents = new List<ISerializedEvent>();

        public IReadOnlyList<ICommitedEvent> Events => _events.AsReadOnly();

        public bool InTransaction;

        public virtual Task<IEnumerable<ICommitedEvent>> GetEventsForwardAsync(Guid aggregateId, int version)
        {
            var events = Events
            .Where(e => e.AggregateId == aggregateId && e.Version > version)
            .OrderBy(e => e.Version)
            .ToList();

            return Task.FromResult<IEnumerable<ICommitedEvent>>(events);
        }

        public void Dispose()
        {
        }

        public void BeginTransaction()
        {
            InTransaction = true;
        }

        public virtual Task CommitAsync()
        {
            if (!InTransaction) throw new InvalidOperationException("You are not in transaction.");

            InTransaction = false;

            _events.AddRange(_uncommitedEvents.Select(InstantiateCommitedEvent));

            _uncommitedEvents.Clear();

            return Task.CompletedTask;
        }

        public virtual void Rollback()
        {
            _uncommitedEvents.Clear();

            InTransaction = false;
        }

        public virtual Task<IEnumerable<ICommitedEvent>> GetAllEventsAsync(Guid aggregateId)
        {
            var events = Events
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Version)
            .ToList();

            return Task.FromResult<IEnumerable<ICommitedEvent>>(events);
        }

        public virtual Task SaveAsync(IEnumerable<ISerializedEvent> collection)
        {
            _uncommitedEvents.AddRange(collection);

            return Task.CompletedTask;
        }

        private static ICommitedEvent InstantiateCommitedEvent(ISerializedEvent serializedEvent)
        {
            return new InMemoryCommitedEvent(serializedEvent.AggregateId, serializedEvent.Version, serializedEvent.SerializedData, serializedEvent.SerializedMetadata);
        }

        internal class InMemoryCommitedEvent : ICommitedEvent
        {
            public Guid AggregateId { get; }
            public int Version { get; }
            public string SerializedData { get; }
            public string SerializedMetadata { get; }

            public InMemoryCommitedEvent(Guid aggregateId, int version, string serializedData, string serializedMetadata)
            {
                AggregateId = aggregateId;
                Version = version;
                SerializedData = serializedData;
                SerializedMetadata = serializedMetadata;
            }

        }

    }
}