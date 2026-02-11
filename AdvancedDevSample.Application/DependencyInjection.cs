using Microsoft.Extensions.DependencyInjection;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Application.Services;

namespace AdvancedDevSample.Application
{
    /// <summary>
    /// Configuration de l'injection de dépendances pour l'application
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Services d'application
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IOrderService, OrderService>();

            // Service d'authentification
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}