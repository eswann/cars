using System;
using Cars.EventSource;

namespace Cars.Testing.Shared.StubApplication.Domain.Foo
{
    public class Foo : Aggregate
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public int DidSomethingCounter { get; private set; }

        public Foo()
        {
        }

        public Foo(Guid id)
        {
            Emit(new FooCreated(id));
        }

        public void ChangeName(string firstname, string lastname)
        {
            Emit(new FullNameChanged(AggregateId, firstname, lastname));
        }

        public void DoSomething()
        {
            Emit(new DidSomething(AggregateId));
        }

        protected override void RegisterEvents()
        {
            SubscribeTo<FooCreated>(e =>
            {
                AggregateId = e.AggregateId;
            });

            SubscribeTo<DidSomething>(e =>
            {
                DidSomethingCounter += 1;
            });

            SubscribeTo<FullNameChanged>(e =>
            {
                FirstName = e.FirstName;
                LastName = e.LastName;
            });
        }
    }
}