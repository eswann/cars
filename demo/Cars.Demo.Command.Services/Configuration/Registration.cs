using Cars.Configuration;
using Cars.Demo.Command.Services.Commands.CreateCart;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.Demo.Command.Services.Configuration
{
    public static class Registration
    {
        public static IServiceCollection RegisterCommandServices(this IServiceCollection services)
        {
            services.RegisterCommandHandlersInAssemblyOf<CreateCartCommand>();

            return services;
        }
    }
}
