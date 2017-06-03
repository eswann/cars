// The MIT License (MIT)
// 
// Copyright (c) 2016 Nelson Corrêa V. Júnior
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cars.Events;
using Cars.EventSource;
using Cars.EventSource.Projections;
using Cars.EventSource.Snapshots;
using Cars.EventSource.Storage;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cars.EventStore.MongoDB
{
    public delegate Task AddOrUpdateProjectionsDelegate(IEnumerable<ISerializedProjection> projections);

    public class MongoEventStore : IEventStore
    {
        protected readonly List<Event> UncommitedEvents = new List<Event>();
        protected readonly List<SnapshotData> UncommitedSnapshots = new List<SnapshotData>();
        protected readonly List<ISerializedProjection> UncommitedProjections = new List<ISerializedProjection>();
        

        public MongoEventStore(IMongoClient client, IMongoEventStoreSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Client = client ?? throw new ArgumentNullException(nameof(client));

            Settings.Validate();
        }

        public IMongoClient Client { get; }

        public IMongoEventStoreSettings Settings { get; }

        public Task SaveSnapshotAsync(ISerializedSnapshot snapshot)
        {
            var snapshotData = Serialize(snapshot);
            UncommitedSnapshots.Add(snapshotData);

            return Task.CompletedTask;
        }

        public async Task<ICommitedSnapshot> GetLatestSnapshotByIdAsync(Guid streamId)
        {
            var db = Client.GetDatabase(Settings.Database);
            var snapshotCollection = db.GetCollection<SnapshotData>(Settings.SnapshotsCollectionName);

            var filter = Builders<SnapshotData>.Filter;
            var sort = Builders<SnapshotData>.Sort;

            var snapshots = await snapshotCollection
                .Find(filter.Eq(x => x.StreamId, streamId))
                .Sort(sort.Descending(x => x.Version))
                .Limit(1)
                .ToListAsync();

            return snapshots.Select(Deserialize).FirstOrDefault();
        }

        public async Task<IEnumerable<ICommitedEvent>> GetEventsForwardAsync(Guid streamId, int version)
        {
            var db = Client.GetDatabase(Settings.Database);
            var collection = db.GetCollection<Event>(Settings.EventsCollectionName);

            var sort = Builders<Event>.Sort;
            var filterBuilder = Builders<Event>.Filter;

            var filter = filterBuilder.Empty
                & filterBuilder.Eq(x => x.StreamId, streamId)
                & filterBuilder.Gt(x => x.Version, version)
                & filterBuilder.Or(filterBuilder.Exists(x => x.Metadata[MetadataKeys.EventIgnore], exists: false), filterBuilder.Eq(x => x.Metadata[MetadataKeys.EventIgnore], false));
            
            var events = await collection
                .Find(filter)
                .Sort(sort.Ascending(x => x.Metadata[MetadataKeys.EventVersion]))
                .ToListAsync();

            return events.Select(Deserialize).ToList();
        }

        public void Dispose()
        {
        }

        public void BeginTransaction()
        {
        }

        public async Task CommitAsync()
        {
            var db = Client.GetDatabase(Settings.Database);

            if (UncommitedSnapshots.Count > 0)
            {
                var snapshotCollection = db.GetCollection<SnapshotData>(Settings.SnapshotsCollectionName);
                await snapshotCollection.InsertManyAsync(UncommitedSnapshots);
            }

            if (UncommitedEvents.Count > 0)
            {
                var eventCollection = db.GetCollection<Event>(Settings.EventsCollectionName);
                await eventCollection.InsertManyAsync(UncommitedEvents);
            }

            if (UncommitedProjections.Count > 0)
            {
                await AddOrUpdateProjectionsAsync(UncommitedProjections);
            }

            Cleanup();
        }

        public void Rollback()
        {
            Cleanup();
        }

        public async Task<IEnumerable<ICommitedEvent>> GetAllEventsAsync(Guid id)
        {
            var db = Client.GetDatabase(Settings.Database);
            var collection = db.GetCollection<Event>(Settings.EventsCollectionName);

            var sort = Builders<Event>.Sort;
            var filterBuilder = Builders<Event>.Filter;

            var filter = filterBuilder.Empty 
                & filterBuilder.Eq(x => x.StreamId, id)
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

        public Task SaveProjectionAsync(ISerializedProjection projection)
        {
            UncommitedProjections.Add(projection);

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
                StreamId = serializedEvent.StreamId,
                Version = serializedEvent.Version,
                EventData = eventData,
                Metadata = metadata
            };

            return @event;
        }

        private void Cleanup()
        {
            UncommitedEvents.Clear();
            UncommitedSnapshots.Clear();
            UncommitedProjections.Clear();
        }

        private ICommitedEvent Deserialize(Event e)
        {
            return MongoCommitedEvent.Create(e);
        }

        private ICommitedSnapshot Deserialize(SnapshotData snapshotData)
        {
            return MongoCommitedSnapshot.Create(snapshotData);
        }

        private SnapshotData Serialize(ISerializedSnapshot serializedSnapshot)
        {
            var eventData = BsonDocument.Parse(serializedSnapshot.SerializedData);
            var metadata = BsonDocument.Parse(serializedSnapshot.SerializedMetadata);
            var id = serializedSnapshot.Metadata.GetValue(MetadataKeys.SnapshotId,
                value => Guid.Parse(value.ToString()));

            var snapshot = new SnapshotData
            {
                Id = id,
                Timestamp = DateTime.UtcNow,
                StreamId = serializedSnapshot.StreamId,
                Version = serializedSnapshot.StreamVersion,
                Data = eventData,
                Metadata = metadata,
            };

            return snapshot;
        }

        protected virtual async Task AddOrUpdateProjectionsAsync(IEnumerable<ISerializedProjection> serializedProjections)
        {
            var db = Client.GetDatabase(Settings.Database);
            var projectionCollection = db.GetCollection<MongoProjection>(Settings.ProjectionsCollectionName);

            var filterBuilder = Builders<MongoProjection>.Filter;

            foreach (var uncommitedProjection in serializedProjections)
            {
                var filter = FilterDefinition<MongoProjection>.Empty
                             & filterBuilder.Eq(e => e.ProjectionId, uncommitedProjection.ProjectionId)
                             & filterBuilder.Eq(e => e.Category, uncommitedProjection.Category);
                
                var mongoProjection = MongoProjection.Create(uncommitedProjection);
                
                await projectionCollection.FindOneAndReplaceAsync(filter, mongoProjection, new FindOneAndReplaceOptions<MongoProjection>
                {
                    IsUpsert = true
                });
            }
        }
    }
}