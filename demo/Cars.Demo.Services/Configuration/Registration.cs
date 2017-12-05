using Cars.Configuration;
using Cars.Demo.Services.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.Demo.Services.Configuration
{
    public static class Registration
    {
        public static IServiceCollection RegisterDemoServices(this IServiceCollection services)
        {
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.RegisterCommandHandlersInAssemblyOf<ProductCatalogSettings>();

            return services;
        }

    }


}
