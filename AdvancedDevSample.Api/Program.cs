using AdvancedDevSample.Application;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Interfaces;
using AdvancedDevSample.Infrastructure;
using AdvancedDevSample.Infrastructure.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =========================
// SERVICES
// =========================

builder.Services.AddControllers();

// 🔐 Configuration JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]
    ?? "AdvancedDevSampleSecretKey2024SecureKeyForJWTGeneration";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// 📘 Swagger + JWT Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AdvancedDevSample.Api",
        Version = "v1"
    });

    // 🔐 Ajout du bouton Authorize
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez : Bearer {votre_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 🏗️ Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// =========================
// PIPELINE
// =========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // 🔄 Création DB + Seed
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AdvancedDevSampleDbContext>();

    try
    {
        await dbContext.Database.EnsureCreatedAsync();

        var supplierRepo = scope.ServiceProvider
            .GetRequiredService<ISupplierRepository>();

        var customerRepo = scope.ServiceProvider
            .GetRequiredService<ICustomerRepository>();

        var userRepo = scope.ServiceProvider
            .GetRequiredService<IUserRepository>();

        // Seed Supplier
        if (!dbContext.Suppliers.Any())
        {
            await supplierRepo.AddAsync(
                new Supplier("Fournisseur Par Défaut", "contact@fournisseur.com"));
        }

        // Seed Customer
        if (!dbContext.Customers.Any())
        {
            await customerRepo.AddAsync(
                new Customer("Client", "Par Défaut", "client@example.com"));
        }

        // Seed Admin
        var adminEmail = "admin@advanceddevsample.com";
        var existingAdmin = await userRepo.GetByEmailAsync(adminEmail);

        if (existingAdmin == null)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");

            var adminUser = new User(
                adminEmail,
                passwordHash,
                "Admin",
                "System",
                "Admin"
            );

            await userRepo.AddAsync(adminUser);
        }

        await dbContext.SaveChangesAsync();

        Console.WriteLine("✓ Base de données initialisée !");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erreur DB : {ex.Message}");
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
