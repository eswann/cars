using System;
using Cars.Attributes;
using Cars.EventSource;
using Cars.EventSource.Projections;
using Cars.UnitTests.Domain.Stubs.Events;

namespace Cars.UnitTests.Domain.Stubs
{
    [ProjectionProvider(typeof(StubStreamProjectionProvider))]
    public class StubStream : Stream
    {
        public string Name { get; private set; }
        public Guid RelatedId { get; private set; }

        private StubStream(Guid newGuid, string name)
        {
            Emit(new StubStreamCreatedEvent(newGuid, name));
        }

        public StubStream()
        {
        }

        public static StubStream Create(string name)
        {
            return new StubStream(Guid.NewGuid(), name);
        }

        public void ChangeName(string name)
        {
            Emit(new NameChangedEvent(Id, name));
        }

        public void DoSomethingWithoutEventSubscription()
        {
            Emit(new NotRegisteredEvent(Id));
        }

        public void Relationship(Guid relatedId)
        {
            Emit(new StubStreamRelatedEvent(Id, relatedId));
        }

        protected override void RegisterEvents()
        {
            SubscribeTo<NameChangedEvent>(x => { Name = x.Name; });
            SubscribeTo<StubStreamCreatedEvent>(x =>
            {
                Id = x.StreamId;
                Name = x.Name;
            });

            SubscribeTo<StubStreamRelatedEvent>(x =>
            {
                RelatedId = x.StubStreamId;
            });
        }

    }

    public class StubStreamProjectionProvider : IProjectionProvider
    {
        public object CreateProjection(IStream stream)
        {
            var target = stream as StubStream;

            return new StubStreamProjection
            {
                Id = target.Id,
                Name = target.Name,
                RelatedId = target.RelatedId
            };
        }
    }

    public class StubStreamProjection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid RelatedId { get; set; }
    }
}