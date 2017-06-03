using System;
using Cars.Events;

namespace Cars.Testing.Shared.StubApplication.Domain.Foo
{
    public class FullNameChanged : DomainEvent
    {
        public string FirstName { get; }
        public string LastName { get; }

        public FullNameChanged(Guid streamId, string firstName, string lastName) : base(streamId)
        {
            LastName = lastName;
            FirstName = firstName;
        }
    }
}