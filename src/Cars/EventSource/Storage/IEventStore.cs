using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cars.Events;
using Cars.EventSource.SerializedEvents;

namespace Cars.EventSource.Storage
{
    /// <summary>
    /// Event Store repository abstraction.
    /// </summary>
    public interface IEventStore : IDisposable
    {
        /// <summary>∑
        /// Start the transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Confirm modifications.
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Revert modifications.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Retrieve all events based on <param name="aggregateId"></param>.
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <returns></returns>
        Task<IEnumerable<ICommitedEvent>> GetAllEventsAsync(Guid aggregateId);

        /// <summary>
        /// Save the events in Event Store.
        /// </summary>
        /// <param name="collection"></param>
        Task SaveAsync(IEnumerable<ISerializedEvent> collection);
    }
}
