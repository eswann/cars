using Cars.Commands;
using System;

namespace Cars.Demo.Services.Commands.CreateCart
{
    public class CreateCartCommand : Command
    {
	    public CreateCartCommand() : base(new Guid()){}

	    public string Name { get; set; }

		public Guid UserId { get; set; }
    }
}
