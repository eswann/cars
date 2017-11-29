using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
	public class CreateFooResponse : IResponse
	{
		public CreateFooResponse(Guid aggregateId)
		{
			AggregateId = aggregateId;
		}

		public Guid AggregateId { get; }
	}
}