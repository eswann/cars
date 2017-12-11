using System;
using Cars.EventSource;

namespace Cars.Events
{
    internal class UncommitedEvent : IUncommitedEvent
    {
        private readonly long _ticks;
        public DateTime CreatedAt => new DateTime(_ticks);
        public IDomainEvent OriginalEvent { get; }
        public Aggregate Aggregate { get; }
        public int Version { get; }

        public UncommitedEvent(Aggregate aggregate, IDomainEvent @event, int version)
        {
            Aggregate = aggregate;
            OriginalEvent = @event;
            Version = version;

            _ticks = DateTime.Now.Ticks;
        }
    }
}