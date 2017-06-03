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
    public class When_stream_have_entities : StreamTestFixture<StubSnapshotStream>
    {
        private const string CategoryName = "Unit";
        private const string CategoryValue = "Snapshot";

        private string newChildName = "New child";

        protected override IEnumerable<IDomainEvent> Given()
        {
            var streamId = Guid.NewGuid();
            
            yield return new StubStreamCreatedEvent(streamId, "Mother");
            yield return new ChildCreatedEvent(streamId, Guid.NewGuid(), "Child 1");
            yield return new ChildCreatedEvent(streamId, Guid.NewGuid(), "Child 2");
        }

        protected override void When()
        {
            StreamRoot.AddEntity(newChildName);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Stream_should_have_3_items()
        {
            StreamRoot.Entities.Should().HaveCount(3);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_be_published_an_event_that_entity_was_created()
        {
            PublishedEvents.Last().Should().BeOfType<ChildCreatedEvent>();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_verify_last_event_properties()
        {
            var childCreatedEvent = PublishedEvents.Last().As<ChildCreatedEvent>();

            childCreatedEvent.Name.Should().Be(newChildName);
        }
    }
}