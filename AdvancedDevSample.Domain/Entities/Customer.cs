using System;
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
            Email = email.Trim().ToLower();
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
            if (!IsActive)
            {
                IsActive = true;
                AddDomainEvent(new CustomerActivatedEvent(Id));
            }
        }

        /// <summary>
        /// Désactive le client
        /// </summary>
        public void Desactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                AddDomainEvent(new CustomerDeactivatedEvent(Id));
            }
        }

        /// <summary>
        /// Met à jour les informations du client
        /// </summary>
        public void Update(string firstName, string lastName, string email)
        {
            ValidateParameters(firstName, lastName, email);

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLower();

            AddDomainEvent(new CustomerUpdatedEvent(Id));
        }

        private void ValidateParameters(string firstName, string lastName, string email)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("Le prénom est requis");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Le nom est requis");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("L'email est requis");

            if (!IsValidEmail(email))
                throw new DomainException("L'email n'est pas valide");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);

                // Vérification plus stricte : doit avoir un @ et un domaine valide
                if (addr.Address != email)
                    return false;

                if (!email.Contains("@"))
                    return false;

                var parts = email.Split('@');
                if (parts.Length != 2)
                    return false;

                if (string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                    return false;

                if (!parts[1].Contains("."))
                    return false;

                if (parts[1].StartsWith(".") || parts[1].EndsWith("."))
                    return false;

                if (parts[1].Contains(".."))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}