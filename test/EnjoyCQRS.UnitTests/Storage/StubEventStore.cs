﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnjoyCQRS.EventSource;
using EnjoyCQRS.EventSource.Snapshots;
using EnjoyCQRS.EventSource.Storage;

namespace EnjoyCQRS.UnitTests.Storage
{
    public class StubEventStore : InMemoryEventStore
    {
        public bool SaveSnapshotMethodCalled { get; private set; }
        public bool GetSnapshotMethodCalled { get; private set; }
        
        public override Task SaveAsync(IEnumerable<ISerializedEvent> collection)
        {
            SaveSnapshotMethodCalled = true;

            return base.SaveAsync(collection);
        }

        public override Task<ICommitedSnapshot> GetLatestSnapshotByIdAsync<TAggregate>(Guid aggregateId)
        {
            GetSnapshotMethodCalled = true;

            return base.GetLatestSnapshotByIdAsync<TAggregate>(aggregateId);
        }
    }
}