using System;
using AdvancedDevSample.Domain.Common;
using AdvancedDevSample.Domain.Entities;

namespace AdvancedDevSample.Domain.Events
{
    /// <summary>
    /// Événement déclenché lors de la création d'un utilisateur
    /// </summary>
    public class UserCreatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Role { get; }

        public UserCreatedEvent(User user)
        {
            UserId = user.Id;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Role = user.Role;
        }
    }

    /// <summary>
    /// Événement déclenché lors de la connexion d'un utilisateur
    /// </summary>
    public class UserLoggedInEvent : DomainEvent
    {
        public Guid UserId { get; }

        public UserLoggedInEvent(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Événement déclenché lors du changement de mot de passe
    /// </summary>
    public class UserPasswordChangedEvent : DomainEvent
    {
        public Guid UserId { get; }

        public UserPasswordChangedEvent(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Événement déclenché lors de la mise à jour du profil
    /// </summary>
    public class UserProfileUpdatedEvent : DomainEvent
    {
        public Guid UserId { get; }

        public UserProfileUpdatedEvent(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Événement déclenché lors de l'activation d'un utilisateur
    /// </summary>
    public class UserActivatedEvent : DomainEvent
    {
        public Guid UserId { get; }

        public UserActivatedEvent(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Événement déclenché lors de la désactivation d'un utilisateur
    /// </summary>
    public class UserDeactivatedEvent : DomainEvent
    {
        public Guid UserId { get; }

        public UserDeactivatedEvent(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Événement déclenché lors du changement de rôle
    /// </summary>
    public class UserRoleChangedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string NewRole { get; }

        public UserRoleChangedEvent(Guid userId, string newRole)
        {
            UserId = userId;
            NewRole = newRole;
        }
    }
}