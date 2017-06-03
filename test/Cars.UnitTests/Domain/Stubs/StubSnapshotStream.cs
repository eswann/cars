using System;
using System.Collections.Generic;
using System.Linq;
using Cars.EventSource;
using Cars.UnitTests.Domain.Stubs.Events;

namespace Cars.UnitTests.Domain.Stubs
{
    public class StubSnapshotStream : SnapshotStream<StubSnapshotStreamSnapshot>
    {
        private readonly List<SimpleEntity> _entities = new List<SimpleEntity>();

        public string Name { get; private set; }

        public IReadOnlyList<SimpleEntity> Entities => _entities.AsReadOnly();

        private StubSnapshotStream(Guid newGuid, string name)
        {
            Emit(new StubStreamCreatedEvent(newGuid, name));
        }

        public StubSnapshotStream()
        {
        }
        
        public static StubSnapshotStream Create(string name)
        {
            return new StubSnapshotStream(Guid.NewGuid(), name);
        }

        public void ChangeName(string name)
        {
            Emit(new NameChangedEvent(Id, name));
        }

        public Guid AddEntity(string entityName)
        {
            var entityId = Guid.NewGuid();

            Emit(new ChildCreatedEvent(Id, entityId, entityName));

            return entityId;
        }

        public void DisableEntity(Guid entityId)
        {
            Emit(new ChildDisabledEvent(Id, entityId));
        }
        
        protected override void RegisterEvents()
        {
            SubscribeTo<StubStreamCreatedEvent>(x =>
            {
                Id = x.StreamId;
                Name = x.Name;
            });

            SubscribeTo<NameChangedEvent>(x =>
            {
                Name = x.Name;
            });

            SubscribeTo<ChildCreatedEvent>(x => _entities.Add(new SimpleEntity(x.StreamId, x.Name)));

            SubscribeTo<ChildDisabledEvent>(x =>
            {
                var entity = _entities.FirstOrDefault(e => e.Id == x.StreamId);
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
