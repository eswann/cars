using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Bar
{
	public class CreateBarResponse
	{
		public CreateBarResponse(Guid barId)
		{
			BarId = barId;
		}

		public Guid BarId { get; }
	}
}