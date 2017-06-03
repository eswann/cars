using Cars.EventSource.Snapshots;

namespace Cars.Testing.Shared.StubApplication.Domain.Foo
{
    public class FooSnapshot : Snapshot
    {
        public int DidSomethingCounter { get; set; }
    }
}