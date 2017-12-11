using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Cars.EventSource.Storage
{
    public class Repository : IRepository
    {
        private readonly ISession _session;
        private readonly ILogger _logger;

        public Repository(ILoggerFactory loggerFactory, ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = loggerFactory?.CreateLogger<Repository>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public void Add<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            _logger.LogDebug($"Called method: {nameof(Repository)}.{nameof(Add)}.");

            _session.Add(aggregate);
        }

        public Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : Aggregate, new()
        {
            _logger.LogDebug($"Called method: {nameof(Repository)}.{nameof(GetByIdAsync)}.");

            return _session.GetByIdAsync<TAggregate>(id);
        }
    }
}
