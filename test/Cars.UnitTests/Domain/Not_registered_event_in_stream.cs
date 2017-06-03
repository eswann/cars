using Cars.EventSource.Exceptions;
using Cars.Testing.Shared.MessageBus;
using Cars.UnitTests.Domain.Stubs;
using Cars.UnitTests.Domain.Stubs.Events;
using FluentAssertions;
using Xunit;

namespace Cars.UnitTests.Domain
{
    public class Not_registered_event_in_stream : StreamTestFixture<StubStream>
    {
        public const string CategoryName = "Unit";
        public const string CategoryValue = "Stream";

        protected override void When()
        {
            StreamRoot.DoSomethingWithoutEventSubscription();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Then_throws_an_exception()
        {
            CaughtException.Should().BeAssignableTo<HandlerNotFound>();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Then_the_event_type_should_be_SomeEvent()
        {
            CaughtException.As<HandlerNotFound>().EventType.Should().BeAssignableTo<NotRegisteredEvent>();
        }
    }
}