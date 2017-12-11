using System.Threading.Tasks;
using Cars.Commands;

namespace Cars.MessageBus
{
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Dispatch the command to the handler.
        /// </summary>
        Task<TResponse> DispatchAsync<TCommand, TResponse>(TCommand command);

        Task<DefaultResponse> DispatchAsync<TCommand>(TCommand command);
    }
}