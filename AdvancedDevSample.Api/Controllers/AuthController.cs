using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;

namespace AdvancedDevSample.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour l'authentification
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Inscription d'un nouvel utilisateur
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            _logger.LogInformation("Requête d'inscription pour: {Email}", registerDto.Email);

<<<<<<< Updated upstream
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.RegisterAsync(registerDto);
            return Ok(response);
=======
            try
            {
                var response = await _authService.RegisterAsync(registerDto);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Échec d'inscription pour {Email}", registerDto.Email);
                return BadRequest(new { message = ex.Message });
            }
>>>>>>> Stashed changes
        }

        /// <summary>
        /// Connexion d'un utilisateur
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Requête de connexion pour: {Email}", loginDto.Email);

<<<<<<< Updated upstream
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
=======
            try
            {
                var response = await _authService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Échec de connexion pour {Email}", loginDto.Email);
                return BadRequest(new { message = ex.Message });
            }
>>>>>>> Stashed changes
        }

        /// <summary>
        /// Récupère les informations de l'utilisateur connecté
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = GetUserEmailFromClaims();

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var user = await _authService.GetCurrentUserAsync(email);
            return Ok(user);
        }

        /// <summary>
        /// Test du token d'authentification
        /// </summary>
        [HttpGet("test")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult TestAuth()
        {
            return Ok(new
            {
                Message = "Authentification réussie",
                Email = GetUserEmailFromClaims(),
                Roles = GetUserRoles()
            });
        }

        private string? GetUserEmailFromClaims()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        }

        private IEnumerable<string> GetUserRoles()
        {
            return User.FindAll(ClaimTypes.Role).Select(c => c.Value);
        }
    }
}