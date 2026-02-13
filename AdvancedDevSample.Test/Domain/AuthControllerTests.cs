using AdvancedDevSample.Api.Controllers;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdvancedDevSample.Test.Domain
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();

            _controller = new AuthController(
                _authServiceMock.Object,
                _loggerMock.Object);
        }

        // ========================= REGISTER =========================

        [Fact]
        public async Task Register_WithValidData_ReturnsOk()
        {
            var dto = new RegisterDto { Email = "test@mail.com", Password = "Password123!" };
            var responseDto = new AuthResponseDto { Token = "fake-token" };

            _authServiceMock
                .Setup(s => s.RegisterAsync(dto))
                .ReturnsAsync(responseDto);

            var result = await _controller.Register(dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(responseDto, okResult.Value);
        }

        [Fact]
        public async Task Register_WithNullDto_ReturnsBadRequest()
        {
            var result = await _controller.Register(null!);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Register_WhenServiceThrows_ReturnsBadRequest()
        {
            var dto = new RegisterDto { Email = "test@mail.com", Password = "Password123!" };

            _authServiceMock
                .Setup(s => s.RegisterAsync(dto))
                .ThrowsAsync(new Exception("Erreur"));

            var result = await _controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ========================= LOGIN =========================

        [Fact]
        public async Task Login_WithValidData_ReturnsOk()
        {
            var dto = new LoginDto { Email = "test@mail.com", Password = "Password123!" };
            var responseDto = new AuthResponseDto { Token = "fake-token" };

            _authServiceMock
                .Setup(s => s.LoginAsync(dto))
                .ReturnsAsync(responseDto);

            var result = await _controller.Login(dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(responseDto, okResult.Value);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var dto = new LoginDto { Email = "test@mail.com", Password = "wrong" };

            _authServiceMock
                .Setup(s => s.LoginAsync(dto))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid"));

            var result = await _controller.Login(dto);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_WithNullDto_ReturnsBadRequest()
        {
            var result = await _controller.Login(null!);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_WhenUnexpectedError_Returns500()
        {
            var dto = new LoginDto { Email = "test@mail.com", Password = "Password123!" };

            _authServiceMock
                .Setup(s => s.LoginAsync(dto))
                .ThrowsAsync(new Exception("Crash"));

            var result = await _controller.Login(dto);

            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        }

        // ========================= GET CURRENT USER =========================

        [Fact]
        public async Task GetCurrentUser_WithValidUser_ReturnsOk()
        {
            var email = "test@mail.com";

            var userDto = new UserDto { Email = email };

            _authServiceMock
                .Setup(s => s.GetCurrentUserAsync(email))
                .ReturnsAsync(userDto);

            SetUserClaims(email);

            var result = await _controller.GetCurrentUser();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(userDto, okResult.Value);
        }

        [Fact]
        public async Task GetCurrentUser_WithMissingEmail_ReturnsUnauthorized()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.GetCurrentUser();

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task GetCurrentUser_WhenUserNotFound_ReturnsNotFound()
        {
            var email = "test@mail.com";

            // ✅ Utiliser Task.FromResult<UserDto?>(null) pour éviter l'erreur de nullable
            _authServiceMock
                .Setup(s => s.GetCurrentUserAsync(email))
                .Returns(() => Task.FromResult<UserDto?>(null));

            SetUserClaims(email);

            var result = await _controller.GetCurrentUser();

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        // ========================= TEST AUTH =========================

        [Fact]
        public void TestAuth_ReturnsOk()
        {
            var email = "test@mail.com";

            SetUserClaims(email, "Admin");

            var result = _controller.TestAuth();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        // ========================= HELPERS =========================

        private void SetUserClaims(string email, string role = "User")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }
    }
}
