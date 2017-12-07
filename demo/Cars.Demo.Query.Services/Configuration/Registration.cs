using Cars.Configuration;
using Cars.Demo.Query.Services.Carts;
using Cars.Demo.Query.Services.Products;
using Cars.Demo.Query.Services.RemovedProducts;
using Microsoft.Extensions.DependencyInjection;

namespace Cars.Demo.Query.Services.Configuration
{
    public static class Registration
    {
        public static IServiceCollection RegisterQueryServices(this IServiceCollection services)
        {
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<ICartRepository, CartRepository>();
            services.AddSingleton<IRemovedProductsRepository, RemovedProductsRepository>();
            services.RegisterEventHandlersInAssemblyOf<ProductCatalogSettings>();

            return services;
        }

    }


}
