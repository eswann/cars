using System;

namespace Cars.Demo.Api.Models
{
    public class CreateCartRequest
    {
		public string Name { get; set; }

		public Guid UserId { get; set; }
    }
}
