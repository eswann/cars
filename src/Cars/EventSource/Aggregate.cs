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
using System.Threading.Tasks;
using Cars.Collections;
using Cars.Events;

namespace Cars.EventSource
{
    public abstract class Aggregate : IAggregate
    {
        private readonly Route<IDomainEvent> _routeEvents = new Route<IDomainEvent>();
        private readonly List<UncommitedEvent> _uncommitedEvents = new List<UncommitedEvent>();

        /// <summary>
        /// Aggregate default constructor.
        /// </summary>
        protected Aggregate()
        {
            RegisterEvents();
        }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid AggregateId { get; protected set; }

        /// <summary>
        /// Current version of the Aggregate.
        /// </summary>
        public int Version { get; protected set; }


        /// <summary>
        /// Collection of <see cref="IDomainEvent"/> that contains uncommited events.
        /// All events that not persisted yet should be here.
        /// </summary>
        public IReadOnlyCollection<IUncommitedEvent> UncommitedEvents => _uncommitedEvents.AsReadOnly();

        /// <summary>
        /// This version is calculated based on Version + Uncommited events count.
        /// </summary>
        public int UncommittedVersion => Version + _uncommitedEvents.Count;

        /// <summary>
        /// Event emitter.
        /// </summary>
        /// <param name="event"></param>
        protected void Emit(IDomainEvent @event)
        {
            ApplyEvent(@event, true);
        }

        /// <summary>
        /// This method is called internaly and you can put all handlers here.
        /// </summary>
        protected abstract void RegisterEvents();

        protected void SubscribeTo<T>(Action<T> action)
            where T : class, IDomainEvent
        {
            _routeEvents.Add(typeof(T), o => action(o as T));
        }


        /// <summary>
        /// Apply the event in Aggregate and store the event in Uncommited list.
        /// The last event applied is the current state of the Aggregate.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="isNew"></param>
        private void ApplyEvent(IDomainEvent @event, bool isNew = false)
        {
            _routeEvents.Handle(@event);

            if (isNew)
            {
                Task.WaitAll(Task.Delay(1));

                _uncommitedEvents.Add(new UncommitedEvent(this, @event, UncommittedVersion + 1));
            }
        }

        /// <summary>
        /// Clear the collection of events that uncommited.
        /// </summary>
        public void ClearUncommitedEvents()
        {
            _uncommitedEvents.Clear();
        }

        /// <summary>
        /// Load the events in the Aggregate.
        /// </summary>
        /// <param name="domainEvents"></param>
        public void LoadFromHistory(CommitedDomainEventCollection domainEvents)
        {
            foreach (var @event in domainEvents)
            {
                ApplyEvent(@event);
            }
        }

        /// <summary>
        /// Update stream's version.
        /// </summary>
        /// <param name="version"></param>
        internal void SetVersion(int version)
        {
            Version = version;
        }

    }
}