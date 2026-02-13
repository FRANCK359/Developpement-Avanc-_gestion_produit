// Customer.cs - ENTITY REFACTORISÉE
using System;
using System.Text.RegularExpressions;
using AdvancedDevSample.Domain.Common;
using AdvancedDevSample.Domain.Events;
using AdvancedDevSample.Domain.Exceptions;

namespace AdvancedDevSample.Domain.Entities
{
    /// <summary>
    /// Représente un client dans le domaine
    /// </summary>
    public class Customer : BaseEntity
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Customer()
        {
            // Constructeur privé pour EF Core
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
        }

        /// <summary>
        /// Initialise une nouvelle instance de Customer
        /// </summary>
        public Customer(string firstName, string lastName, string email)
        {
            ValidateParameters(firstName, lastName, email);

            Id = Guid.NewGuid();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = NormalizeEmail(email);
            IsActive = true;
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new CustomerCreatedEvent(this));
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Active le client
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;

            IsActive = true;
            AddDomainEvent(new CustomerActivatedEvent(Id));
        }

        /// <summary>
        /// Désactive le client
        /// </summary>
        public void Desactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            AddDomainEvent(new CustomerDeactivatedEvent(Id));
        }

        /// <summary>
        /// Met à jour les informations du client
        /// </summary>
        public void Update(string firstName, string lastName, string email)
        {
            ValidateParameters(firstName, lastName, email);

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = NormalizeEmail(email);

            AddDomainEvent(new CustomerUpdatedEvent(Id));
        }

        private static void ValidateParameters(string firstName, string lastName, string email)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("Le prénom est requis");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Le nom est requis");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("L'email est requis");

            if (!IsValidEmail(email))
                throw new DomainException($"L'email '{email}' n'est pas valide");
        }

        private static bool IsValidEmail(string email)
        {
<<<<<<< Updated upstream
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
=======
            if (string.IsNullOrWhiteSpace(email))
>>>>>>> Stashed changes
                return false;

            email = email.Trim();

            // Vérification de base avec regex
            if (!EmailRegex.IsMatch(email))
                return false;

            // Vérifications supplémentaires
            var parts = email.Split('@');
            if (parts.Length != 2)
                return false;

            var localPart = parts[0];
            var domain = parts[1];

            // Vérifier la partie locale
            if (string.IsNullOrWhiteSpace(localPart) || localPart.Length > 64)
                return false;

            // Vérifier le domaine
            if (string.IsNullOrWhiteSpace(domain) || domain.Length > 255)
                return false;

            if (domain.StartsWith(".") || domain.EndsWith(".") || domain.Contains(".."))
                return false;

            return true;
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}