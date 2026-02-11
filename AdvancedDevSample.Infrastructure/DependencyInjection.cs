using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AdvancedDevSample.Domain.Interfaces;
using AdvancedDevSample.Infrastructure.DbContext;
using AdvancedDevSample.Infrastructure.Repositories;

namespace AdvancedDevSample.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Vérifier si on est en mode test
            var isTesting = AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName?.Contains("Test") == true);

            if (isTesting)
            {
                // En mode test, utiliser InMemory
                services.AddDbContext<AdvancedDevSampleDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            }
            else
            {
                // En production, utiliser SQL Server
                services.AddDbContext<AdvancedDevSampleDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        sqlServerOptions =>
                        {
                            sqlServerOptions.MigrationsAssembly(typeof(AdvancedDevSampleDbContext).Assembly.FullName);
                            sqlServerOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: System.TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        }));
            }

            // Repositories
            services.AddScoped<IProductRepository, EfProductRepository>();
            services.AddScoped<ICustomerRepository, EfCustomerRepository>();
            services.AddScoped<ISupplierRepository, EfSupplierRepository>();
            services.AddScoped<IOrderRepository, EfOrderRepository>();
            services.AddScoped<IUserRepository, EfUserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}