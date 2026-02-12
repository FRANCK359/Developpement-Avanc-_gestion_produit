using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AdvancedDevSample.Test.Integration
{
    public class OrdersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _clientWithAuth;
        private readonly HttpClient _clientWithoutAuth;

        public OrdersControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            // CLIENT SANS AUTHENTIFICATION - Pour tester Unauthorized
            _clientWithoutAuth = factory.CreateClient();

            // CLIENT AVEC AUTHENTIFICATION - Pour tester OK
            _clientWithAuth = factory.CreateClient();
            _clientWithAuth.DefaultRequestHeaders.Add(
                "Authorization",
                $"{TestAuthHandler.AuthenticationScheme} test-token");
        }

        [Fact]
        public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
        {
            // ARRANGE - Utiliser le client sans authentification
            var client = _clientWithoutAuth;

            // ACT
            var response = await client.GetAsync("/api/orders");

            // ASSERT
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_WithAuth_ReturnsOk()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            // ACT
            var response = await client.GetAsync("/api/orders");

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_Invalid_ReturnsNotFound()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;
            var invalidId = Guid.NewGuid();

            // ACT
            var response = await client.GetAsync($"/api/orders/{invalidId}");

            // ASSERT
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}