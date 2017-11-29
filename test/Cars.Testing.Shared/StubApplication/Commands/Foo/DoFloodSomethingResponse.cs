using System;

namespace Cars.Testing.Shared.StubApplication.Commands.Foo
{
	public class DoFloodSomethingResponse
	{
		public DoFloodSomethingResponse(Guid itemId)
		{
			ItemId = itemId;
		}

		public Guid ItemId { get; }
	}
}