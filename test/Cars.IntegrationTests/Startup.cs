using Cars.Configuration;
using Cars.Core;
using Cars.Testing.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace Cars.IntegrationTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO: Move it to another class and simplify for consumer

	        services.AddCars();
            services.AddTransient<ITextSerializer, JsonTextSerializer>();

            services.RegisterCommandHandlersInAssemblyOf<FooAssembler>();
	        services.RegisterEventHandlersInAssemblyOf<FooAssembler>();

            services.AddRouting();
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
