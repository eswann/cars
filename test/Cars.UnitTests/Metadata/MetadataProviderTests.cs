using System.Linq;
using Cars.EventSource;
using Cars.MetadataProviders;
using Cars.UnitTests.Domain.Stubs;
using FluentAssertions;
using Xunit;

namespace Cars.UnitTests.Metadata
{
    public class MetadataProviderTests
    {
        public const string CategoryName = "Unit";
        public const string CategoryValue = "Metadata";

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Event_MetadataProvider()
        {
            var stubStream = StubStream.Create("Test");

            var metadataProvider = new EventTypeMetadataProvider();

            var metadata = stubStream.UncommitedEvents.SelectMany(e => metadataProvider.Provide(stubStream, e.OriginalEvent, EventSource.Metadata.Empty));

            metadata.Count().Should().Be(2);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Stream_MetadataProvider()
        {
            var stubStream = StubStream.Create("Test");

            var metadataProvider = new StreamTypeMetadataProvider();

            var metadata = stubStream.UncommitedEvents.SelectMany(e => metadataProvider.Provide(stubStream, e.OriginalEvent, EventSource.Metadata.Empty));

            metadata.Count().Should().Be(3);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void CorrelationId_MetadataProvider()
        {
            var stubStream = StubStream.Create("Test");
            stubStream.ChangeName("Test 1");
            stubStream.ChangeName("Test 2");

            var metadataProvider = new CorrelationIdMetadataProvider();

            var metadatas = stubStream.UncommitedEvents.SelectMany(e => metadataProvider.Provide(stubStream, e.OriginalEvent, EventSource.Metadata.Empty));

            metadatas.Select(e => e.Value).Distinct().Count().Should().Be(1);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_take_event_name_based_on_attribute()
        {
            var stubStream = StubStream.Create("Test");
            var metadataProvider = new EventTypeMetadataProvider();
            var metadatas = stubStream.UncommitedEvents.SelectMany(e => metadataProvider.Provide(stubStream, e.OriginalEvent, EventSource.Metadata.Empty));

            var metadata = new EventSource.Metadata(metadatas);

            metadata.GetValue(MetadataKeys.EventName).Should().Be("StubCreated");
        }
    }
}