using Cars.EventSource.Snapshots;
using Cars.UnitTests.Domain.Stubs;
using FluentAssertions;
using Xunit;

namespace Cars.UnitTests.Snapshoting
{
    public class When_create_snapshot
    {
        public const string CategoryName = "Unit";
        public const string CategoryValue = "Snapshot";

        private readonly ISnapshot _snapshot;
        
        public When_create_snapshot()
        {
            var stubSnapshotStream = StubSnapshotAggregate.Create("Superman");
            stubSnapshotStream.ChangeName("Batman");
            stubSnapshotStream.AddEntity("entity 1");
            var entityId = stubSnapshotStream.AddEntity("entity 2");

            stubSnapshotStream.DisableEntity(entityId);

            _snapshot = ((ISnapshotAggregate) stubSnapshotStream).CreateSnapshot();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_create_an_snapshot_object()
        {
            _snapshot.Should().BeOfType<StubSnapshotStreamSnapshot>();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_verify_snapshot_properties()
        {
            var snapshot = (StubSnapshotStreamSnapshot) _snapshot;

            snapshot.Name.Should().Be("Batman");
            snapshot.SimpleEntities.Count.Should().Be(2);
            snapshot.SimpleEntities[1].Enabled.Should().BeFalse();
        }
    }
}