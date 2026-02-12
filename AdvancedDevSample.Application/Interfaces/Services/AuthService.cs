using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Interfaces;

namespace AdvancedDevSample.Application.Services
{
    /// <summary>
    /// Implémentation du service d'authentification
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Tentative d'inscription pour l'email: {Email}", registerDto.Email);

            // Validation des mots de passe
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                throw new ValidationException("Les mots de passe ne correspondent pas");
            }

            // Validation de l'email
            if (string.IsNullOrWhiteSpace(registerDto.Email))
            {
                throw new ValidationException("L'email est requis");
            }

            if (!IsValidEmail(registerDto.Email))
            {
                throw new ValidationException("L'email n'est pas valide");
            }

            // Vérifier si l'email existe déjà
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new ValidationException($"Un utilisateur avec l'email {registerDto.Email} existe déjà");
            }

            // Hasher le mot de passe
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Créer l'utilisateur
            var user = new User(
                registerDto.Email,
                passwordHash,
                registerDto.FirstName,
                registerDto.LastName,
                "User"
            );

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Générer le token
            var token = GenerateJwtToken(user);

            _logger.LogInformation("Utilisateur inscrit avec succès: {UserId}", user.Id);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Inscription réussie",
                Token = token,
                User = MapToUserDto(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Tentative de connexion pour l'email: {Email}", loginDto.Email);

            // Validation de l'email
            if (string.IsNullOrWhiteSpace(loginDto.Email))
            {
                throw new ValidationException("L'email est requis");
            }

            // Vérifier si l'utilisateur existe
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Tentative de connexion avec email inexistant: {Email}", loginDto.Email);
                throw new ValidationException("Email ou mot de passe incorrect");
            }

            // Vérifier le mot de passe
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Tentative de connexion avec mot de passe incorrect pour: {Email}", loginDto.Email);
                throw new ValidationException("Email ou mot de passe incorrect");
            }

            // Vérifier si le compte est actif
            if (!user.IsActive)
            {
                _logger.LogWarning("Tentative de connexion sur compte inactif: {Email}", loginDto.Email);
                throw new ValidationException("Votre compte est désactivé. Contactez l'administrateur.");
            }

            // Mettre à jour la dernière connexion
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Générer le token
            var token = GenerateJwtToken(user);

            _logger.LogInformation("Connexion réussie pour: {UserId}", user.Id);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Connexion réussie",
                Token = token,
                User = MapToUserDto(user)
            };
        }

        public async Task<UserDto> GetCurrentUserAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ValidationException("L'email est requis");
            }

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new NotFoundException($"Utilisateur avec l'email {email} non trouvé");
            }

            return MapToUserDto(user);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "AdvancedDevSampleSecretKey2024SecureKeyForJWTGeneration";
            var issuer = jwtSettings["Issuer"] ?? "AdvancedDevSample.Api";
            var audience = jwtSettings["Audience"] ?? "AdvancedDevSample.Client";
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("given_name", user.FirstName),
                new Claim("family_name", user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}