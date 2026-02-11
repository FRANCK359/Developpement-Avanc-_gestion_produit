using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;

namespace AdvancedDevSample.Application.Interfaces.Services
{
    /// <summary>
    /// Service d'authentification
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Inscription d'un nouvel utilisateur
        /// </summary>
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// Connexion d'un utilisateur
        /// </summary>
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Récupère les informations de l'utilisateur connecté
        /// </summary>
        Task<UserDto> GetCurrentUserAsync(string email);
    }
}