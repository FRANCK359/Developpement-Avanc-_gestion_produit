using Xunit;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;

namespace AdvancedDevSample.Test.Integration
{
    public class ProductsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private void AddTestAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme, "dummy-token");
        }

        [Fact]
        public async Task GetAll_WithAuth_ReturnsSuccessStatusCode()
        {
            AddTestAuthHeader();
            var response = await _client.GetAsync("/api/products");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
        {
            // Supprimer le header pour simuler absence d'auth
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync("/api/products");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            AddTestAuthHeader();
            var invalidId = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/products/{invalidId}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_WithValidData_ReturnsCreated()
        {
            AddTestAuthHeader();

            var supplierDto = new CreateSupplierDto
            {
                Name = $"Fournisseur Test {Guid.NewGuid()}",
                ContactEmail = $"contact_{Guid.NewGuid()}@test.com"
            };

            var supplierResponse = await _client.PostAsJsonAsync("/api/suppliers", supplierDto);
            supplierResponse.EnsureSuccessStatusCode();
            var supplier = await supplierResponse.Content.ReadFromJsonAsync<SupplierDto>();

            var productDto = new CreateProductDto
            {
                Name = $"Produit Test {Guid.NewGuid()}",
                Description = "Description test",
                Price = 99.99m,
                SupplierId = supplier!.Id
            };

            var response = await _client.PostAsJsonAsync("/api/products", productDto);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
            Assert.NotNull(createdProduct);
            Assert.Equal(productDto.Name, createdProduct.Name);
        }
    }
}
