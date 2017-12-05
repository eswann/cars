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
using Cars.Collections;
using Cars.Events;

namespace Cars.EventSource
{
    public abstract class Projection : IProjection
    {
        protected readonly Route<IDomainEvent> RouteEvents = new Route<IDomainEvent>();

        /// <summary>
        /// Aggregate default constructor.
        /// </summary>
        protected Projection()
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
        /// This method is called internaly and you can put all handlers here.
        /// </summary>
        protected virtual void RegisterEvents()
        {
        }

        protected void SubscribeTo<T>(Action<T> action)
            where T : class, IDomainEvent
        {
            RouteEvents.Add(typeof(T), o => action(o as T));
        }

        /// <summary>
        /// Apply the event in Mutator.
        /// The last event applied is the current state of the Mutator.
        /// </summary>
        /// <param name="event"></param>
        private void ApplyEvent(IDomainEvent @event)
        {
            RouteEvents.Handle(@event);
        }

        /// <summary>
        /// Load the events in the Mutator.
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