using System.Threading.Tasks;

namespace Cars.Commands
{
    public interface ICommandHandler<in TCommand, TResponse>
    {
        Task<TResponse> ExecuteAsync(TCommand command);
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, DefaultResponse>
    {
    }
}
