using Cars.Commands;

namespace Cars.Demo.Services.Carts.Commands.CreateCart
{
    public interface ICreateCartHandler : ICommandHandler<CreateCartCommand, CreateCartResponse>
    {
        
    }
}