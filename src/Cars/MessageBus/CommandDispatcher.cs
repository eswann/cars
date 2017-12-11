using System;
using System.Threading.Tasks;
using Cars.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.MessageBus
{
    public class CommandDispatcher : ICommandDispatcher
    {
	    private readonly IServiceProvider _serviceProvider;

	    public CommandDispatcher(IServiceProvider serviceProvider)
	    {
		    _serviceProvider = serviceProvider;
	    }

		public Task<TResponse> DispatchAsync<TCommand, TResponse>(TCommand command)
        {
		        var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResponse>>();
		        return handler.ExecuteAsync(command);
        }

        public Task<DefaultResponse> DispatchAsync<TCommand>(TCommand command)
        {
            return DispatchAsync<TCommand, DefaultResponse>(command);
        }
    }
}