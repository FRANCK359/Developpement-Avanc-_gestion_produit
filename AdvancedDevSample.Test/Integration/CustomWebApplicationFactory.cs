using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using AdvancedDevSample.Infrastructure.DbContext;
using AdvancedDevSample.Api;

namespace AdvancedDevSample.Test.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
<<<<<<< Updated upstream
        protected override IHost CreateHost(IHostBuilder builder)
=======
        private const string TestDatabaseName = "TestDatabase";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
>>>>>>> Stashed changes
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(ConfigureTestServices);
        }

<<<<<<< Updated upstream
            builder.ConfigureServices(services =>
            {
                // 1. SUPPRIMER TOUS LES SERVICES EF EXISTANTS
                var descriptorsToRemove = services
                    .Where(d => d.ServiceType.Namespace?.Contains("EntityFramework") == true ||
                               d.ServiceType == typeof(DbContextOptions) ||
                               d.ServiceType == typeof(DbContextOptions<AdvancedDevSampleDbContext>))
                    .ToList();

                foreach (var descriptor in descriptorsToRemove)
                {
                    services.Remove(descriptor);
                }

                // 2. AJOUTER IN-MEMORY DATABASE SANS PROVIDER CONFLIT
                services.AddDbContext<AdvancedDevSampleDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

                // 3. SUPPRIMER L'AUTHENTIFICATION JWT
                var authDescriptors = services
                    .Where(d => d.ServiceType.FullName?.Contains("JwtBearer") == true ||
                               d.ServiceType.FullName?.Contains("Authentication") == true)
                    .ToList();

                foreach (var descriptor in authDescriptors)
                {
                    services.Remove(descriptor);
                }

                // 4. AJOUTER L'AUTHENTIFICATION DE TEST
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                    options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                    options.DefaultForbidScheme = TestAuthHandler.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme, options => { });
            });

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AdvancedDevSampleDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                InitializeTestData(db);
            });
        }

        private void InitializeTestData(AdvancedDevSampleDbContext db)
        {
            // Ajouter un fournisseur de test
            var supplier = new Domain.Entities.Supplier(
                "Fournisseur Test",
                "contact@test.com"
            );
            db.Suppliers.Add(supplier);

            // Ajouter un client de test
            var customer = new Domain.Entities.Customer(
                "Client",
                "Test",
                "client.test@example.com"
            );
            db.Customers.Add(customer);

            // Ajouter un produit de test
            var product = new Domain.Entities.Product(
                "Produit Test",
                "Description du produit test",
                99.99m,
                supplier.Id
            );
            db.Products.Add(product);

            db.SaveChanges();
=======
        private void ConfigureTestServices(IServiceCollection services)
        {
            ReplaceProductionDatabaseWithInMemory(services);
            ReplaceProductionAuthenticationWithTest(services);
            InitializeTestDatabase(services);
        }

        private void ReplaceProductionDatabaseWithInMemory(IServiceCollection services)
        {
            RemoveService<DbContextOptions<AdvancedDevSampleDbContext>>(services);

            services.AddDbContext<AdvancedDevSampleDbContext>(options =>
                options.UseInMemoryDatabase(TestDatabaseName));
        }

        private void ReplaceProductionAuthenticationWithTest(IServiceCollection services)
        {
            RemoveService<IAuthenticationService>(services);

            services.AddAuthentication(ConfigureTestAuthentication)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme,
                    _ => { });
        }

        private void ConfigureTestAuthentication(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
            options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
            options.DefaultForbidScheme = TestAuthHandler.AuthenticationScheme;
        }

        private void InitializeTestDatabase(IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AdvancedDevSampleDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        private void RemoveService<TService>(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
>>>>>>> Stashed changes
        }
    }
}