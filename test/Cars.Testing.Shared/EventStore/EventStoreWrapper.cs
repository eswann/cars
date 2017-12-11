using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cars.Events;
using Cars.EventSource.SerializedEvents;
using Cars.EventSource.Storage;

namespace Cars.Testing.Shared.EventStore
{
    public class EventStoreWrapper : IEventStore
    {
        public EventStoreMethods CalledMethods;

        private readonly IEventStore _eventStore;

        public EventStoreWrapper(IEventStore eventStore)
        {
            _eventStore = eventStore;

            CalledMethods &= EventStoreMethods.Ctor;
        }

        public void Dispose()
        {
            _eventStore.Dispose();

            CalledMethods |= EventStoreMethods.Dispose;
        }

        public void BeginTransaction()
        {
            _eventStore.BeginTransaction();

            CalledMethods |= EventStoreMethods.BeginTransaction;
        }

        public async Task CommitAsync()
        {
            await _eventStore.CommitAsync();

            CalledMethods |= EventStoreMethods.CommitAsync;
        }

        public void Rollback()
        {
            _eventStore.Rollback();

            CalledMethods |= EventStoreMethods.Rollback;
        }

        public async Task<IList<ICommitedEvent>> GetEventsByAggregateId(Guid aggregateId)
        {
            var result = await _eventStore.GetEventsByAggregateId(aggregateId).ConfigureAwait(false);

            CalledMethods |= EventStoreMethods.GetEventsByAggregateId;

            return result;
        }

        public async Task<IList<ICommitedEvent>> GetEventsByTypeAsync(IList<Type> eventTypes, DateTime fromTimestamp)
        {
            var result = await _eventStore.GetEventsByTypeAsync(eventTypes, fromTimestamp).ConfigureAwait(false);

            CalledMethods |= EventStoreMethods.GetEventsByTypeAsync;

            return result;
        }

        public async Task SaveAsync(IEnumerable<ISerializedEvent> collection)
        {
            await _eventStore.SaveAsync(collection).ConfigureAwait(false);

            CalledMethods |= EventStoreMethods.SaveAsync;
        }

    }
}