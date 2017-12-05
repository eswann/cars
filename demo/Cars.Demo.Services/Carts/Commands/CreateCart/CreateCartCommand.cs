using System;
using Cars.Commands;

namespace Cars.Demo.Services.Carts.Commands.CreateCart
{
    public class CreateCartCommand : Command
    {
	    public CreateCartCommand() : base(new Guid()){}

		public string UserId { get; set; }
    }
}
