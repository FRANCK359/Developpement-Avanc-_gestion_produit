using AdvancedDevSample.Domain.Common;
using AdvancedDevSample.Domain.Exceptions;
using System;

namespace AdvancedDevSample.Domain.Entities
{
    /// <summary>
    /// Représente un utilisateur du système
    /// </summary>
    public class User : BaseEntity
    {
        private User()
        {
            Email = string.Empty;
            PasswordHash = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Role = string.Empty;
        }

        public User(string email, string passwordHash, string firstName, string lastName, string role = "User")
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("L'email est requis");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Le mot de passe est requis");

            Id = Guid.NewGuid();
            Email = email.Trim().ToLower();
            PasswordHash = passwordHash;
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Role = role;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            LastLoginAt = null;
        }

        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new DomainException("Le nouveau mot de passe est requis");

            PasswordHash = newPasswordHash;
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void ChangeRole(string newRole)
        {
            Role = newRole;
        }
    }
}