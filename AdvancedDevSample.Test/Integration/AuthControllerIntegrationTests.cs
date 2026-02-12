using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdvancedDevSample.Api;
using AdvancedDevSample.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AdvancedDevSample.Test.Integration
{
    public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOkAndToken()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = $"newuser.{Guid.NewGuid()}@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.NotNull(authResponse);
            Assert.True(authResponse!.Success);
            Assert.NotNull(authResponse.Token);
            Assert.NotNull(authResponse.User);
            Assert.Equal(registerDto.Email.ToLower(), authResponse.User!.Email);
        }

        [Fact]
        public async Task Register_WithInvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "invalid-email",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithMismatchedPasswords_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = $"test.{Guid.NewGuid()}@example.com",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword!",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var email = $"duplicate.{Guid.NewGuid()}@example.com";
            var registerDto1 = new RegisterDto
            {
                Email = email,
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerDto1);

            var registerDto2 = new RegisterDto
            {
                Email = email,
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "Jane",
                LastName = "Smith"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto2);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkAndToken()
        {
            // Arrange
            var email = $"login.{Guid.NewGuid()}@example.com";
            var registerDto = new RegisterDto
            {
                Email = email,
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            var loginDto = new LoginDto
            {
                Email = email,
                Password = "Password123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.NotNull(authResponse);
            Assert.True(authResponse!.Success);
            Assert.NotNull(authResponse.Token);
            Assert.NotNull(authResponse.User);
            Assert.Equal(email.ToLower(), authResponse.User!.Email);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var email = $"login.{Guid.NewGuid()}@example.com";
            var registerDto = new RegisterDto
            {
                Email = email,
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            var loginDto = new LoginDto
            {
                Email = email,
                Password = "WrongPassword!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithNonExistentEmail_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}