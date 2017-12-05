using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
    public class DoSomethingResponse
	{
	    public DoSomethingResponse(Guid id)
	    {
		    AggregateId = id;
	    }

		public Guid AggregateId { get; }

    }
}