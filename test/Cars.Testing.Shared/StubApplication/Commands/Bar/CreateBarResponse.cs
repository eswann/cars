using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Bar
{
	public class CreateBarResponse : IResponse
	{
		public CreateBarResponse(Guid barId)
		{
			BarId = barId;
		}

		public Guid BarId { get; }

		public Guid AggregateId => BarId;
	}
}