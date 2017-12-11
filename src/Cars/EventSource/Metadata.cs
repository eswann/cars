using System;
using System.Collections.Generic;
using System.Linq;

namespace Cars.EventSource
{
    public class Metadata : Dictionary<string, object>, IMetadata
    {
        public static Metadata Empty => new Metadata();

        public Metadata(IEnumerable<KeyValuePair<string, object>> keyValuePairs) : base(keyValuePairs.ToDictionary(e => e.Key, e => e.Value))
        {
        }

        public Metadata()
        {   
        }

        public object GetValue(string key) => this[key];

        public T GetValue<T>(string key, Func<object, T> converter)
        {
            var value = this[key];
            return converter(value);
        }
    }

    public struct MetadataKeys
    {
        public const string StreamTypeFullname = "streamTypeFullname";
        public const string AggregateId = "aggregateId";
        public const string StreamSequenceNumber = "streamSequenceNumber";

        public const string EventId = "eventId";
        public const string EventClrType = "eventClrType";
        public const string EventName = "eventName";
        public const string EventVersion = "eventVersion";
        public const string EventIgnore = "ignore";

        public const string CorrelationId = "correlationId";
    }

    public struct EventDataKeys
    {
        public const string Metadata = "metadata";
        public const string Timestamp = "timestamp";
    }
}