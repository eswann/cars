using Cars.EventSource;
using Cars.EventSource.Snapshots;
using Cars.UnitTests.Domain.Stubs;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cars.UnitTests.Snapshoting
{
    public class IntervalSnapshotStrategyTests
    {
        private const string CategoryName = "Unit";
        private const string CategoryValue = "IntervalSnapshotStrategy";

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void When_stream_type_have_support_snapshoting()
        {
            var snapshotStreamType = typeof(StubSnapshotAggregate);
            
            var itervalSnapshotStrategy = new IntervalSnapshotStrategy();
            var hasSupport = itervalSnapshotStrategy.CheckSnapshotSupport(snapshotStreamType);

            hasSupport.Should().BeTrue();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void When_stream_type_doesnt_have_support_snapshoting()
        {
            var snapshotStreamType = typeof(StubAggregate);

            var defaultSnapshotStrategy = new IntervalSnapshotStrategy();
            var hasSupport = defaultSnapshotStrategy.CheckSnapshotSupport(snapshotStreamType);

            hasSupport.Should().BeFalse();
        }

        [Trait(CategoryName, CategoryValue)]
        [Theory]
        [InlineData(100, 100, true)]
        [InlineData(200, 100, true)]
        [InlineData(101, 100, false)]
        [InlineData(115, 15, false)]
        [InlineData(105, 15, true)]
        [InlineData(2, 5, false)]
        public void Should_make_snapshot(int streamEventVersion, int snapshotInterval, bool expected)
        {
            var itervalSnapshotStrategy = new IntervalSnapshotStrategy(snapshotInterval);

            var snapshotStreamMock = new Mock<ISnapshotAggregate>();
            snapshotStreamMock.Setup(e => e.Sequence).Returns(streamEventVersion);

            var snapshotStream = snapshotStreamMock.Object;
            
            var makeSnapshot = itervalSnapshotStrategy.ShouldMakeSnapshot(snapshotStream);

            makeSnapshot.Should().Be(expected);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_not_make_snapshot()
        {
            var itervalSnapshotStrategy = new IntervalSnapshotStrategy();

            var streamMock = new Mock<IAggregate>();
            var stream = streamMock.Object;
          
            var makeSnapshot = itervalSnapshotStrategy.ShouldMakeSnapshot(stream);

            makeSnapshot.Should().BeFalse();
        }
    }
}