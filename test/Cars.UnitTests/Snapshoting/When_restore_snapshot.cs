using System;
using Cars.EventSource.Snapshots;
using Cars.UnitTests.Domain.Stubs;
using FluentAssertions;
using Xunit;

namespace Cars.UnitTests.Snapshoting
{
    public class When_restore_snapshot
    {
        public const string CategoryName = "Unit";
        public const string CategoryValue = "Snapshot";

        private readonly StubSnapshotStreamSnapshot _snapshot;
        private readonly StubSnapshotStream _stubStream;

        public When_restore_snapshot()
        {
            var streamId = Guid.NewGuid();
            var version = 1;

            _snapshot = new StubSnapshotStreamSnapshot
            {
                Name = "Coringa",
            };
            
            _stubStream = new StubSnapshotStream();
            ((ISnapshotStream)_stubStream).Restore(new SnapshotRestore(streamId, version, _snapshot, EventSource.Metadata.Empty));
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public void Should_set_stream_properties()
        {
            _stubStream.Name.Should().Be(_snapshot.Name);

            _stubStream.Version.Should().Be(1);
        }
    }
}