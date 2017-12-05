using Cars.Demo.Services.Configuration;
using Cars.EventStore.MongoDB;
using Cars.EventStore.MongoDB.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Cars.Demo.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoEventStoreSettings>(Configuration.GetSection("EventStoreSettings"));
	        services.AddSingleton<IMongoEventStoreSettings>(provider => provider.GetService<IOptions<MongoEventStoreSettings>>().Value);

            services.Configure<ProductCatalogSettings>(Configuration.GetSection("ProductCatalogSettings"));
            services.AddSingleton<IProductCatalogSettings>(provider => provider.GetService<IOptions<ProductCatalogSettings>>().Value);

            services.AddCarsMongo();

            services.RegisterDemoServices();

            // Add framework services.
            services.AddMvc();

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("MyPolicy");
            app.UseMvc();
        }
    }
}
