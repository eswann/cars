using System;
using System.Collections.Generic;
using System.Linq;
using Cars.Events;
using Cars.Testing.Shared.MessageBus;
using Cars.UnitTests.Domain.Stubs;
using Cars.UnitTests.Domain.Stubs.Events;
using FluentAssertions;
using Xunit;

namespace Cars.UnitTests.Domain
{
    public class When_stream_modify_an_entity : StreamTestFixture<StubSnapshotStream>
    {
        private const string CategoryName = "Unit";
        private const string CategoryValue = "Snapshot";
        
        private readonly ChildCreatedEvent _entity2 = new ChildCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), "Child 2");

        protected override IEnumerable<IDomainEvent> Given()
        {
            yield return new StubStreamCreatedEvent(Guid.NewGuid(), "Mother");
            yield return new ChildCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), "Child 1");
            yield return _entity2;
        }

        protected override void When()
        {
            StreamRoot.DisableEntity(_entity2.EntityId);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Stream_should_have_2_items()
        {
            StreamRoot.Entities.Should().HaveCount(2);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_be_published_an_event_that_entity_was_disabled()
        {
            PublishedEvents.Last().Should().BeOfType<ChildDisabledEvent>();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_verify_last_event_properties()
        {
            var childDisabledEvent = PublishedEvents.Last().As<ChildDisabledEvent>();

            childDisabledEvent.EntityId.Should().Be(_entity2.EntityId);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Entity2_should_be_disabled()
        {
            StreamRoot.Entities[1].Enabled.Should().BeFalse();
        }
    }
}