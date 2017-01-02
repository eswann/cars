using EnjoyCQRS.Commands;
using EnjoyCQRS.Configuration;
using EnjoyCQRS.Core;
using EnjoyCQRS.Events;
using EnjoyCQRS.MessageBus;
using EnjoyCQRS.UnitTests.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace EnjoyCQRS.IntegrationTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICommandDispatcher, CustomCommandDispatcher>();
            services.AddTransient<IEventRouter, CustomEventRouter>();
            services.AddTransient<ITextSerializer, JsonTextSerializer>();

            services.Scan(e =>
                e.FromAssemblyOf<FooAssembler>()
                    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces());

            services.Scan(e =>
                e.FromAssemblyOf<FooAssembler>()
                    .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
                    .AsImplementedInterfaces());

            services.AddEnjoyCqrs();

            services.AddRouting();
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
