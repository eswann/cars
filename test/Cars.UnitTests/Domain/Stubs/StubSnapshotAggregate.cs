using System;
using System.Collections.Generic;
using System.Linq;
using Cars.EventSource;
using Cars.EventSource.Snapshots;
using Cars.UnitTests.Domain.Stubs.Events;

namespace Cars.UnitTests.Domain.Stubs
{
    public class StubSnapshotAggregate : SnapshotAggregate<StubSnapshotStreamSnapshot>
    {
        private readonly List<SimpleEntity> _entities = new List<SimpleEntity>();

        public string Name { get; private set; }

        public IReadOnlyList<SimpleEntity> Entities => _entities.AsReadOnly();

        private StubSnapshotAggregate(Guid newGuid, string name)
        {
            Emit(new StubStreamCreatedEvent(newGuid, name));
        }

        public StubSnapshotAggregate()
        {
        }
        
        public static StubSnapshotAggregate Create(string name)
        {
            return new StubSnapshotAggregate(Guid.NewGuid(), name);
        }

        public void ChangeName(string name)
        {
            Emit(new NameChangedEvent(AggregateId, name));
        }

        public Guid AddEntity(string entityName)
        {
            var entityId = Guid.NewGuid();

            Emit(new ChildCreatedEvent(AggregateId, entityId, entityName));

            return entityId;
        }

        public void DisableEntity(Guid entityId)
        {
            Emit(new ChildDisabledEvent(AggregateId, entityId));
        }
        
        protected override void RegisterEvents()
        {
            SubscribeTo<StubStreamCreatedEvent>(x =>
            {
                AggregateId = x.AggregateId;
                Name = x.Name;
            });

            SubscribeTo<NameChangedEvent>(x =>
            {
                Name = x.Name;
            });

            SubscribeTo<ChildCreatedEvent>(x => _entities.Add(new SimpleEntity(x.EntityId, x.Name)));

            SubscribeTo<ChildDisabledEvent>(x =>
            {
                var entity = _entities.FirstOrDefault(e => e.Id == x.EntityId);
                entity?.Disable();
            });
        }

        protected override StubSnapshotStreamSnapshot CreateSnapshot()
        {
            return new StubSnapshotStreamSnapshot
            {
                Name = Name,
                SimpleEntities = _entities
            };
        }

        protected override void RestoreFromSnapshot(StubSnapshotStreamSnapshot snapshot)
        {
            Name = snapshot.Name;

            _entities.Clear();
            _entities.AddRange(snapshot.SimpleEntities);
        }

    }
}
