using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using AdvancedDevSample.Infrastructure.DbContext;
using AdvancedDevSample.Api;
using System.Linq;

namespace AdvancedDevSample.Test.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // 1. Supprimer les DbContext existants
                var descriptorsToRemove = services
                    .Where(d => d.ServiceType.Namespace?.Contains("EntityFramework") == true ||
                               d.ServiceType == typeof(DbContextOptions) ||
                               d.ServiceType == typeof(DbContextOptions<AdvancedDevSampleDbContext>))
                    .ToList();

                foreach (var descriptor in descriptorsToRemove)
                    services.Remove(descriptor);

                // 2. Ajouter InMemoryDatabase
                services.AddDbContext<AdvancedDevSampleDbContext>(options =>
                    options.UseInMemoryDatabase("TestDatabase"), ServiceLifetime.Scoped, ServiceLifetime.Scoped);

                // 3. Supprimer l'auth JWT existante
                var authDescriptors = services
                    .Where(d => d.ServiceType.FullName?.Contains("JwtBearer") == true ||
                               d.ServiceType.FullName?.Contains("Authentication") == true)
                    .ToList();

                foreach (var descriptor in authDescriptors)
                    services.Remove(descriptor);

                // 4. Ajouter TestAuthHandler
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                    options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                    options.DefaultForbidScheme = TestAuthHandler.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme, options => { });

                // 5. CRÉER LE SCOPE ET INITIALISER LES DONNÉES ICI
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AdvancedDevSampleDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                InitializeTestData(db);
            });

            return base.CreateHost(builder);
        }

        // Supprimer ConfigureWebHost complètement ou le garder vide
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            // Ne rien mettre ici
        }

        private void InitializeTestData(AdvancedDevSampleDbContext db)
        {
            // Fournisseur de test
            var supplier = new Domain.Entities.Supplier("Fournisseur Test", "contact@test.com");
            db.Suppliers.Add(supplier);

            // Client de test
            var customer = new Domain.Entities.Customer("Client", "Test", "client.test@example.com");
            db.Customers.Add(customer);

            // Produit de test
            var product = new Domain.Entities.Product("Produit Test", "Description du produit test", 99.99m, supplier.Id);
            db.Products.Add(product);

            // UTILISATEUR DE TEST POUR AUTH
            var user = new Domain.Entities.User(
                TestAuthHandler.TestUserEmail,
                BCrypt.Net.BCrypt.HashPassword("Test123!"),
                "Test",
                "User",
                "User"
            );
            db.Users.Add(user);

            db.SaveChanges();
        }
    }
}