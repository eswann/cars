using System;
using System.Threading.Tasks;

namespace Cars.EventSource.Storage
{
    /// <summary>
    /// Represents an abstraction where an instance of <see cref="Aggregate"/> will be persisted.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Retrieves an <typeparam name="TAggregate"></typeparam> based on your unique identifier property.
        /// </summary>
        Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : Aggregate, new();

        /// <summary>
        /// Add an instance of <typeparam name="TAggregate"></typeparam> in repository.
        /// </summary>
        void Add<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate;
    }
}
