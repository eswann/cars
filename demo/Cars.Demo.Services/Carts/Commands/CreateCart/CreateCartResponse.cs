using System;
using Cars.Commands;

namespace Cars.Demo.Services.Carts.Commands.CreateCart
{
	public class CreateCartResponse
	{
		public CreateCartResponse(Guid cartId)
		{
			CartId = cartId;
		}

		public Guid CartId { get; }
	}
}