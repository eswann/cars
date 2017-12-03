using System;
using Cars.Commands;

namespace Cars.Demo.Services.Commands.CreateCart
{
	public class CreateCartResponse : IResponse
	{
		public CreateCartResponse(Guid cartId)
		{
			CartId = cartId;
		}

		public Guid CartId { get; }
		public Guid AggregateId => CartId;
	}
}