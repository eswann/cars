using System.Threading.Tasks;

namespace Cars.Commands
{
	public interface IDefaultCommandHandler<in TCommand> where TCommand : ICommand
	{
		Task<IResponse> ExecuteAsync(TCommand command);
	}
}