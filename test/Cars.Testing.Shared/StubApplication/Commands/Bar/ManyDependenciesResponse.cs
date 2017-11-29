using System;

namespace Cars.Testing.Shared.StubApplication.Commands.Bar
{
	public class ManyDependenciesResponse
	{
		public ManyDependenciesResponse(Guid itemId)
		{
			ItemId = itemId;
		}

		public Guid ItemId { get; }
	}
}