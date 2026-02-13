using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AdvancedDevSample.Api.Controllers;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdvancedDevSample.Test.Components
{
    /// <summary>
    /// Tests de composant pour le contrôleur AuthController
    /// Ces tests vérifient le comportement du contrôleur en isolation
    /// </summary>
    public class AuthControllerComponentTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerComponentTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        }

        #region Register Tests

        [Fact]
        public async Task Register_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                FirstName = "Test",
                LastName = "User"
            };

            var expectedResponse = new AuthResponseDto
            {
                Success = true,
                Message = "Inscription réussie",
                Token = "jwt-token",
                User = new UserDto
                {
                    Id = Guid.NewGuid(),
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName!,
                    LastName = registerDto.LastName!,
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _mockAuthService
                .Setup(x => x.RegisterAsync(registerDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.Equal(expectedResponse.Success, response.Success);
            Assert.Equal(expectedResponse.Message, response.Message);
            Assert.Equal(expectedResponse.Token, response.Token);
            Assert.Equal(expectedResponse.User!.Email, response.User!.Email);

            _mockAuthService.Verify(x => x.RegisterAsync(registerDto), Times.Once);
        }

        [Fact]
        public async Task Register_WithNullDto_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Register(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Données invalides.", badRequestResult.Value);
            _mockAuthService.Verify(x => x.RegisterAsync(It.IsAny<RegisterDto>()), Times.Never);
        }

        [Fact]
        public async Task Register_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Email est requis");
            var registerDto = new RegisterDto();

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            _mockAuthService.Verify(x => x.RegisterAsync(It.IsAny<RegisterDto>()), Times.Never);
        }

        [Fact]
        public async Task Register_WhenExceptionThrown_ReturnsBadRequestWithMessage()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "existing@example.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            _mockAuthService
                .Setup(x => x.RegisterAsync(registerDto))
                .ThrowsAsync(new Exception("Un utilisateur avec cet email existe déjà"));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorResponse = badRequestResult.Value;
            var message = errorResponse?.GetType().GetProperty("message")?.GetValue(errorResponse)?.ToString();

            Assert.Equal("Un utilisateur avec cet email existe déjà", message);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            var expectedResponse = new AuthResponseDto
            {
                Success = true,
                Message = "Connexion réussie",
                Token = "jwt-token",
                User = new UserDto
                {
                    Id = Guid.NewGuid(),
                    Email = loginDto.Email,
                    FirstName = "Test",
                    LastName = "User",
                    Role = "User",
                    IsActive = true,
                    LastLoginAt = DateTime.UtcNow
                }
            };

            _mockAuthService
                .Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.Equal(expectedResponse.Success, response.Success);
            Assert.Equal(expectedResponse.Message, response.Message);
            Assert.Equal(expectedResponse.Token, response.Token);

            _mockAuthService.Verify(x => x.LoginAsync(loginDto), Times.Once);
        }

        [Fact]
        public async Task Login_WithNullDto_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Login(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Données invalides.", badRequestResult.Value);
            _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginDto>()), Times.Never);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            _mockAuthService
                .Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new UnauthorizedAccessException("Email ou mot de passe incorrect"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var errorResponse = unauthorizedResult.Value;
            var message = errorResponse?.GetType().GetProperty("message")?.GetValue(errorResponse)?.ToString();

            Assert.Equal("Email ou mot de passe incorrect", message);
        }

        [Fact]
        public async Task Login_WhenOtherExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            _mockAuthService
                .Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new Exception("Erreur base de données"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var errorResponse = statusCodeResult.Value;
            var message = errorResponse?.GetType().GetProperty("message")?.GetValue(errorResponse)?.ToString();

            Assert.Equal("Une erreur interne est survenue.", message);
        }

        #endregion

        #region GetCurrentUser Tests

        [Fact]
        public async Task GetCurrentUser_WithAuthenticatedUser_ReturnsOkResult()
        {
            // Arrange
            var userEmail = "test@example.com";
            var expectedUser = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = userEmail,
                FirstName = "Test",
                LastName = "User",
                Role = "User",
                IsActive = true
            };

            // Simuler l'utilisateur authentifié avec ClaimTypes.Email
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, userEmail)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _mockAuthService
                .Setup(x => x.GetCurrentUserAsync(userEmail))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(expectedUser.Email, user.Email);
            Assert.Equal(expectedUser.FirstName, user.FirstName);
            Assert.Equal(expectedUser.LastName, user.LastName);

            _mockAuthService.Verify(x => x.GetCurrentUserAsync(userEmail), Times.Once);
        }

        [Fact]
        public async Task GetCurrentUser_WithAuthenticatedUserUsingJwtClaim_ReturnsOkResult()
        {
            // Arrange
            var userEmail = "test@example.com";
            var expectedUser = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = userEmail,
                FirstName = "Test",
                LastName = "User",
                Role = "User",
                IsActive = true
            };

            // Simuler l'utilisateur authentifié avec JwtRegisteredClaimNames.Email
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, userEmail)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _mockAuthService
                .Setup(x => x.GetCurrentUserAsync(userEmail))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(expectedUser.Email, user.Email);

            _mockAuthService.Verify(x => x.GetCurrentUserAsync(userEmail), Times.Once);
        }

        [Fact]
        public async Task GetCurrentUser_WithoutAuthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
            _mockAuthService.Verify(x => x.GetCurrentUserAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetCurrentUser_WithEmptyEmail_ReturnsUnauthorized()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, "")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
            _mockAuthService.Verify(x => x.GetCurrentUserAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetCurrentUser_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userEmail = "nonexistent@example.com";
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, userEmail)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Solution: Utiliser ReturnsAsync avec l'opérateur null!
            _mockAuthService
                .Setup(x => x.GetCurrentUserAsync(userEmail))
                .ReturnsAsync((UserDto)null!);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var errorResponse = notFoundResult.Value;
            var message = errorResponse?.GetType().GetProperty("message")?.GetValue(errorResponse)?.ToString();

            Assert.Equal("Utilisateur introuvable.", message);
        }

        #endregion

        #region TestAuth Tests

        [Fact]
        public void TestAuth_WithAuthenticatedUser_ReturnsOkResult()
        {
            // Arrange
            var userEmail = "test@example.com";
            var roles = new[] { "User", "Admin" };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userEmail),
                new Claim(ClaimTypes.Role, roles[0]),
                new Claim(ClaimTypes.Role, roles[1])
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = _controller.TestAuth();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            var message = response?.GetType().GetProperty("Message")?.GetValue(response)?.ToString();
            var email = response?.GetType().GetProperty("Email")?.GetValue(response)?.ToString();
            var responseRoles = response?.GetType().GetProperty("Roles")?.GetValue(response) as List<string>;

            Assert.Equal("Authentification réussie", message);
            Assert.Equal(userEmail, email);
            Assert.NotNull(responseRoles);
            Assert.Equal(roles.Length, responseRoles.Count);
            Assert.Contains(roles[0], responseRoles);
            Assert.Contains(roles[1], responseRoles);
        }

        [Fact]
        public void TestAuth_WithAuthenticatedUserUsingJwtClaim_ReturnsOkResult()
        {
            // Arrange
            var userEmail = "test@example.com";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, userEmail)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = _controller.TestAuth();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            var message = response?.GetType().GetProperty("Message")?.GetValue(response)?.ToString();
            var email = response?.GetType().GetProperty("Email")?.GetValue(response)?.ToString();

            Assert.Equal("Authentification réussie", message);
            Assert.Equal(userEmail, email);
        }

        [Fact]
        public void TestAuth_WithoutRoles_ReturnsOkResultWithEmptyRolesList()
        {
            // Arrange
            var userEmail = "test@example.com";

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, userEmail)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = _controller.TestAuth();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            var responseRoles = response?.GetType().GetProperty("Roles")?.GetValue(response) as List<string>;

            Assert.NotNull(responseRoles);
            Assert.Empty(responseRoles);
        }

        #endregion
    }
}