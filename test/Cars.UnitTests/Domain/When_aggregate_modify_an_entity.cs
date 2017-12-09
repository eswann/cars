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
    public class When_stream_modify_an_entity : StreamTestFixture<StubEntityAggregate>
    {
        private const string _categoryName = "Unit";
        private const string _categoryValue = "Snapshot";

	    private readonly Guid _aggregateId = Guid.NewGuid();
        private ChildCreatedEvent _event2;

        protected override IEnumerable<IDomainEvent> Given()
        {
	        _event2 = new ChildCreatedEvent(_aggregateId, Guid.NewGuid(), "Child 2");

			yield return new StubAggregateCreatedEvent(_aggregateId, "Mother");
            yield return new ChildCreatedEvent(_aggregateId, Guid.NewGuid(), "Child 1");
	        yield return _event2;
        }

        protected override void When()
        {
            AggregateRoot.DisableEntity(_event2.EntityId);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Stream_should_have_2_items()
        {
            AggregateRoot.Entities.Should().HaveCount(2);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Should_be_published_an_event_that_entity_was_disabled()
        {
            PublishedEvents.Last().Should().BeOfType<ChildDisabledEvent>();
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Should_verify_last_event_properties()
        {
            var childDisabledEvent = PublishedEvents.Last().As<ChildDisabledEvent>();

            childDisabledEvent.AggregateId.Should().Be(_event2.AggregateId);
	        childDisabledEvent.EntityId.Should().Be(_event2.EntityId);
		}

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Entity2_should_be_disabled()
        {
            AggregateRoot.Entities[1].Enabled.Should().BeFalse();
        }
    }
}