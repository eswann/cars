namespace Cars.Demo.Command.Services.Commands.CreateCart
{
    public class CreateCartCommand
    {
        public CreateCartCommand(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}
