﻿using System;
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
    public class When_aggregate_has_entities : StreamTestFixture<StubEntityAggregate>
    {
        private const string _categoryName = "Unit";
        private const string _categoryValue = "Snapshot";

        private string newChildName = "New child";

        protected override IEnumerable<IDomainEvent> Given()
        {
            var aggregateId = Guid.NewGuid();
            
            yield return new StubAggregateCreatedEvent(aggregateId, "Mother");
            yield return new ChildCreatedEvent(aggregateId, Guid.NewGuid(), "Child 1");
            yield return new ChildCreatedEvent(aggregateId, Guid.NewGuid(), "Child 2");
        }

        protected override void When()
        {
            AggregateRoot.AddEntity(newChildName);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Stream_should_have_3_items()
        {
            AggregateRoot.Entities.Should().HaveCount(3);
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Should_be_published_an_event_that_entity_was_created()
        {
            PublishedEvents.Last().Should().BeOfType<ChildCreatedEvent>();
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Should_verify_last_event_properties()
        {
            var childCreatedEvent = PublishedEvents.Last().As<ChildCreatedEvent>();

            childCreatedEvent.Name.Should().Be(newChildName);
        }
    }
}