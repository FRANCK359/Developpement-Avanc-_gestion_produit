using Xunit;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using AdvancedDevSample.Api;
using AdvancedDevSample.Application.DTOs;

namespace AdvancedDevSample.Test.Integration
{
    /// <summary>
    /// Tests d'intégration pour le ProductsController
    /// </summary>
    public class ProductsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ProductsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private async Task<string> GetAuthTokenAsync()
        {
            // Créer un utilisateur de test
            var registerDto = new RegisterDto
            {
                Email = "test@integration.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                FirstName = "Test",
                LastName = "Integration"
            };

            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerDto),
                Encoding.UTF8,
                "application/json");

            await _client.PostAsync("/api/auth/register", registerContent);

            // Se connecter
            var loginDto = new LoginDto
            {
                Email = "test@integration.com",
                Password = "Test123!"
            };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/auth/login", loginContent);
            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return authResponse?.Token ?? string.Empty;
        }

        [Fact]
        public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_WithAuth_ReturnsSuccessStatusCode()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var invalidId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/products/{invalidId}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_WithValidData_ReturnsCreated()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Créer d'abord un fournisseur
            var supplierDto = new CreateSupplierDto
            {
                Name = "Fournisseur Test",
                ContactEmail = "contact@test.com"
            };

            var supplierContent = new StringContent(
                JsonSerializer.Serialize(supplierDto),
                Encoding.UTF8,
                "application/json");

            var supplierResponse = await _client.PostAsync("/api/suppliers", supplierContent);
            supplierResponse.EnsureSuccessStatusCode();
            var supplier = await supplierResponse.Content.ReadFromJsonAsync<SupplierDto>();

            // Créer le produit
            var productDto = new CreateProductDto
            {
                Name = "Produit Test Integration",
                Description = "Description test",
                Price = 99.99m,
                SupplierId = supplier!.Id
            };

            var productContent = new StringContent(
                JsonSerializer.Serialize(productDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/products", productContent);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
            Assert.NotNull(createdProduct);
            Assert.Equal(productDto.Name, createdProduct.Name);
        }
    }
}