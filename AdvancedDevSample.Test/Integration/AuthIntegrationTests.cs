using AdvancedDevSample.Api;
using AdvancedDevSample.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AdvancedDevSample.Test.Integration
{
    /// <summary>
    /// Tests d'intégration pour l'authentification
    /// </summary>
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _clientWithAuth;
        private readonly HttpClient _clientWithoutAuth;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;

            _clientWithoutAuth = _factory.CreateClient();

            _clientWithAuth = _factory.CreateClient();
            _clientWithAuth.DefaultRequestHeaders.Add(
                "Authorization",
                $"{TestAuthHandler.AuthenticationScheme} test-token");
        }

        [Fact]
        public async Task GetCurrentUser_WithAuth_ReturnsOk()
        {
            // Act
            var response = await _clientWithAuth.GetAsync("/api/auth/me");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            Assert.NotNull(user);
            Assert.Equal(TestAuthHandler.TestUserEmail, user.Email);
        }

        [Fact]
        public async Task GetCurrentUser_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _clientWithoutAuth.GetAsync("/api/auth/me");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task TestEndpoint_WithAuth_ReturnsOk()
        {
            // Act
            var response = await _clientWithAuth.GetAsync("/api/auth/test");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task TestEndpoint_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _clientWithoutAuth.GetAsync("/api/auth/test");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}