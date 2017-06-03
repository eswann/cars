using Cars.EventSource;
using Cars.EventSource.Snapshots;
using Cars.UnitTests.Domain.Stubs;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cars.UnitTests.Snapshoting
{
    public class DefaultSnapshotStrategyTests
    {
        private const string CategoryName = "Unit";
        private const string CategoryValue = "DefaultSnapshotStrategy";

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void When_stream_type_have_support_snapshoting()
        {
            var snapshotStreamType = typeof(StubSnapshotStream);
            
            var defaultSnapshotStrategy = new DefaultSnapshotStrategy();
            var hasSupport = defaultSnapshotStrategy.CheckSnapshotSupport(snapshotStreamType);

            hasSupport.Should().BeTrue();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void When_stream_type_doesnt_have_support_snapshoting()
        {
            var snapshotStreamType = typeof(StubStream);

            var defaultSnapshotStrategy = new DefaultSnapshotStrategy();
            var hasSupport = defaultSnapshotStrategy.CheckSnapshotSupport(snapshotStreamType);

            hasSupport.Should().BeFalse();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_make_snapshot()
        {
            var defaultSnapshotStrategy = new DefaultSnapshotStrategy();

            var snapshotStream = Mock.Of<ISnapshotStream>();
            
            var makeSnapshot = defaultSnapshotStrategy.ShouldMakeSnapshot(snapshotStream);

            makeSnapshot.Should().BeTrue();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_not_make_snapshot()
        {
            var defaultSnapshotStrategy = new DefaultSnapshotStrategy();

            var stream = Mock.Of<IStream>();
          
            var makeSnapshot = defaultSnapshotStrategy.ShouldMakeSnapshot(stream);

            makeSnapshot.Should().BeFalse();
        }
    }
}