using System;
using System.Threading.Tasks;
using AdvancedDevSample.Domain.Entities;

namespace AdvancedDevSample.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository d'utilisateurs
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> ExistsByEmailAsync(string email);
        Task<int> SaveChangesAsync();
    }
}