using Cars.Configuration;
using Cars.Demo.Query.Services.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.Demo.Query.Services.Configuration
{
    public static class Registration
    {
        public static IServiceCollection RegisterQueryServices(this IServiceCollection services)
        {
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.RegisterEventHandlersInAssemblyOf<ProductCatalogSettings>();

            return services;
        }

    }


}
