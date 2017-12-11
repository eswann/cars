using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cars.Events;
using Cars.EventSource;
using Cars.EventSource.SerializedEvents;
using Cars.EventSource.Storage;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cars.EventStore.MongoDB
{

    public class MongoEventStore : IEventStore
    {
        protected readonly List<Event> UncommitedEvents = new List<Event>();
        

        public MongoEventStore(IMongoClient client, IMongoEventStoreSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Client = client ?? throw new ArgumentNullException(nameof(client));

            Settings.Validate();
        }

        public IMongoClient Client { get; }

        public IMongoEventStoreSettings Settings { get; }

        public void Dispose()
        {
        }

        public void BeginTransaction()
        {
        }

        public async Task CommitAsync()
        {
            var db = Client.GetDatabase(Settings.Database);

            if (UncommitedEvents.Count > 0)
            {
                var eventCollection = db.GetCollection<Event>(Settings.EventsCollectionName);
                await eventCollection.InsertManyAsync(UncommitedEvents);
            }

            Cleanup();
        }

        public void Rollback()
        {
            Cleanup();
        }

        public async Task<IEnumerable<ICommitedEvent>> GetAllEventsAsync(Guid aggregateId)
        {
            var db = Client.GetDatabase(Settings.Database);
            var collection = db.GetCollection<Event>(Settings.EventsCollectionName);

            var sort = Builders<Event>.Sort;
            var filterBuilder = Builders<Event>.Filter;

            var filter = filterBuilder.Empty 
                & filterBuilder.Eq(x => x.AggregateId, aggregateId)
                & filterBuilder.Or(filterBuilder.Exists(x => x.Metadata[MetadataKeys.EventIgnore], exists: false), filterBuilder.Eq(x => x.Metadata[MetadataKeys.EventIgnore], false));

            var events = await collection
                .Find(filter)
                .Sort(sort.Ascending(x => x.Metadata[MetadataKeys.EventVersion]))
                .ToListAsync();

            return events.Select(Deserialize).ToList();
        }

        public Task SaveAsync(IEnumerable<ISerializedEvent> collection)
        {
            var eventList = collection.Select(Serialize);
            UncommitedEvents.AddRange(eventList);

            return Task.CompletedTask;
        }

        private Event Serialize(ISerializedEvent serializedEvent)
        {
            var eventData = BsonDocument.Parse(serializedEvent.SerializedData);
            var metadata = BsonDocument.Parse(serializedEvent.SerializedMetadata);
            var id = serializedEvent.Metadata.GetValue(MetadataKeys.EventId, value => Guid.Parse(value.ToString()));
            var eventType = serializedEvent.Metadata.GetValue(MetadataKeys.EventName, value => value.ToString());

            var @event = new Event
            {
                Id = id,
                Timestamp = DateTime.UtcNow,
                EventType = eventType,
                AggregateId = serializedEvent.AggregateId,
                Version = serializedEvent.Version,
                EventData = eventData,
                Metadata = metadata
            };

            return @event;
        }

        private void Cleanup()
        {
            UncommitedEvents.Clear();
        }

        private ICommitedEvent Deserialize(Event e)
        {
            return MongoCommitedEvent.Create(e);
        }

    }
}