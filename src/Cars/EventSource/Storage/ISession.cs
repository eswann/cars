using System;
using System.Threading.Tasks;

namespace Cars.EventSource.Storage
{
    /// <summary>
    /// Represents an abstraction where stream events will be persisted.
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Retrieves an <typeparam name="TAggregate"></typeparam> based on your unique identifier property.
        /// </summary>
        Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : Aggregate, new();
        
        /// <summary>
        /// Add an instance of <typeparam name="TAggregate"></typeparam>.
        /// </summary>
        void Add<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate;

        /// <summary>
        /// Begin the transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Save the modifications.
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Revert modifications.
        /// </summary>
        void Rollback();

    }
}