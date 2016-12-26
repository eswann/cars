using EnjoyCQRS.Defaults;
using EnjoyCQRS.Handlers;
using EnjoyCQRS.UnitTests.Shared.StubApplication.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EnjoyCQRS.IntegrationTests
{
    public class Startup2
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDispatcher, Dispatcher>();

            services.AddTransient<IAsyncMessageHandler<TransientBarAsyncMessage>, TransientBarAsyncMessageHandler>();
            services.AddScoped<IAsyncMessageHandler<ScopedBarAsyncMessage>, ScopedBarAsyncMessageHandler>();

            services.AddRouting();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}