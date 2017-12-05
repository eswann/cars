using System;
using Cars.Commands;

namespace Cars.Demo.Services.Carts.Commands.CreateCart
{
    public class CreateCartCommand : ICommand
    {
        public CreateCartCommand(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}
