using AdvancedDevSample.Application.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AdvancedDevSample.Test.Integration
{
    public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient Client;

        public IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            Client = factory.CreateClient();
        }

        protected async Task<string> GetAuthTokenAsync()
        {
            var email = $"test{Guid.NewGuid()}@integration.com";

            var registerDto = new RegisterDto
            {
                Email = email,
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            await Client.PostAsJsonAsync("/api/auth/register", registerDto);

            var loginDto = new LoginDto
            {
                Email = email,
                Password = registerDto.Password
            };

            var response = await Client.PostAsJsonAsync("/api/auth/login", loginDto);
            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

            return authResponse!.Token;
        }

        protected async Task AuthenticateAsync()
        {
            var token = await GetAuthTokenAsync();

            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
