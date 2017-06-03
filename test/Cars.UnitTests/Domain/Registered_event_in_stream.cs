using System.Linq;
using Cars.Testing.Shared.MessageBus;
using Cars.UnitTests.Domain.Stubs;
using Cars.UnitTests.Domain.Stubs.Events;
using FluentAssertions;
using Xunit;

namespace Cars.UnitTests.Domain
{
    public class Registered_event_in_stream : StreamTestFixture<StubStream>
    {
        public const string CategoryName = "Unit";
        public const string CategoryValue = "Stream";

        protected override void When()
        {
            StreamRoot = StubStream.Create("Heinsenberg");
            StreamRoot.ChangeName("Walter White");
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
            PublishedEvents.Last().As<NameChangedEvent>().Name.Should().Be(StreamRoot.Name);
        }
    }
}