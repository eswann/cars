using EnjoyCQRS.EventSource.Snapshots;

namespace EnjoyCQRS.Testing.Shared.StubApplication.Domain.FooAggregate
{
    public class FooSnapshot : Snapshot
    {
        public int DidSomethingCounter { get; set; }
    }
}