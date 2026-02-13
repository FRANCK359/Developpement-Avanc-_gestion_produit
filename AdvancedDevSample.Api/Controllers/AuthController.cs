using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
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
            if (registerDto == null)
                return BadRequest("Données invalides.");

            _logger.LogInformation("Requête d'inscription pour: {Email}", registerDto.Email);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _authService.RegisterAsync(registerDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erreur lors de l'inscription pour {Email}", registerDto.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Connexion d'un utilisateur
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
                return BadRequest("Données invalides.");

            _logger.LogInformation("Requête de connexion pour: {Email}", loginDto.Email);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _authService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Échec de connexion pour {Email}", loginDto.Email);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la connexion");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Une erreur interne est survenue." });
            }
        }

        /// <summary>
        /// Récupère les informations de l'utilisateur connecté
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            var user = await _authService.GetCurrentUserAsync(email);

            if (user == null)
                return NotFound(new { message = "Utilisateur introuvable." });

            return Ok(user);
        }

        /// <summary>
        /// Test du token d'authentification
        /// </summary>
        [Authorize]
        [HttpGet("test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult TestAuth()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            var roles = User.FindAll(ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList();

            return Ok(new
            {
                Message = "Authentification réussie",
                Email = email,
                Roles = roles
            });
        }
    }
}
