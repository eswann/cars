using System.Linq;
using Cars.Testing.Shared.MessageBus;
using Cars.UnitTests.Domain.Stubs;
using Cars.UnitTests.Domain.Stubs.Events;
using FluentAssertions;
using Xunit;

namespace Cars.UnitTests.Domain
{
    public class Registered_event_in_stream : StreamTestFixture<StubAggregate>
    {
        public const string CategoryName = "Unit";
        public const string CategoryValue = "Aggregate";

        protected override void When()
        {
            AggregateRoot = StubAggregate.Create("Heinsenberg");
            AggregateRoot.ChangeName("Walter White");
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Then_some_event_should_be_published()
        {
            PublishedEvents.Last().Should().BeAssignableTo<NameChangedEvent>();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Then_verify_name_property()
        {
            PublishedEvents.Last().As<NameChangedEvent>().Name.Should().Be(AggregateRoot.Name);
        }
    }
}