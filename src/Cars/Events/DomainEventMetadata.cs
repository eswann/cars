using System;

namespace Cars.Events
{
    public class DomainEventMetadata
    {
        public DomainEventMetadata() { }

        public DomainEventMetadata(DateTime timestamp, int sequence)
        {
            Timestamp = timestamp;
            Sequence = sequence;
        }

        public DateTime Timestamp { get; protected set; }

        public int Sequence { get; protected set; }
    }
}