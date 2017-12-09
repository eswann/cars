using System;
using System.Collections.Generic;
using System.Linq;
using Cars.EventSource;
using Cars.UnitTests.Domain.Stubs.Events;

namespace Cars.UnitTests.Domain.Stubs
{
    public class StubEntityAggregate : Aggregate
    {
        private readonly List<SimpleEntity> _entities = new List<SimpleEntity>();

        public string Name { get; private set; }

        public IReadOnlyList<SimpleEntity> Entities => _entities.AsReadOnly();

        private StubEntityAggregate(Guid newGuid, string name)
        {
            Emit(new StubAggregateCreatedEvent(newGuid, name));
        }

        public StubEntityAggregate()
        {
        }
        
        public static StubEntityAggregate Create(string name)
        {
            return new StubEntityAggregate(Guid.NewGuid(), name);
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
            SubscribeTo<StubAggregateCreatedEvent>(x =>
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

    }
}
