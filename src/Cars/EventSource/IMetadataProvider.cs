using System.Collections.Generic;
using Cars.Events;

namespace Cars.EventSource
{
    public interface IMetadataProvider
    {
        /// <summary>
        /// Provides a collection of key pair value that will be used to generate event metadata on the save moment.
        /// </summary>
        /// <typeparam name="TAggregate"></typeparam>
        /// <param name="aggregate"></param>
        /// <param name="event"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, object>> Provide<TAggregate>(TAggregate aggregate, IDomainEvent @event, IMetadata metadata)
            where TAggregate : IAggregate;
    }
}