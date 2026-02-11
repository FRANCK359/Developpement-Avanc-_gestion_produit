using System;
using AdvancedDevSample.Domain.Common;
using AdvancedDevSample.Domain.Events;
using AdvancedDevSample.Domain.Exceptions;

namespace AdvancedDevSample.Domain.Entities
{
    /// <summary>
    /// Représente un fournisseur dans le domaine
    /// </summary>
    public class Supplier : BaseEntity
    {
        private Supplier()
        {
            // Constructeur privé pour EF Core
            Name = string.Empty;
            ContactEmail = string.Empty;
        }

        /// <summary>
        /// Initialise une nouvelle instance de Supplier
        /// </summary>
        public Supplier(string name, string contactEmail)
        {
            ValidateParameters(name, contactEmail);

            Id = Guid.NewGuid();
            Name = name.Trim();
            ContactEmail = contactEmail.Trim().ToLower();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new SupplierCreatedEvent(this));
        }

        public string Name { get; private set; }
        public string ContactEmail { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Active le fournisseur
        /// </summary>
        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                AddDomainEvent(new SupplierActivatedEvent(Id));
            }
        }

        /// <summary>
        /// Désactive le fournisseur
        /// </summary>
        public void Desactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                AddDomainEvent(new SupplierDeactivatedEvent(Id));
            }
        }

        /// <summary>
        /// Met à jour les informations du fournisseur
        /// </summary>
        public void Update(string name, string contactEmail)
        {
            ValidateParameters(name, contactEmail);

            Name = name.Trim();
            ContactEmail = contactEmail.Trim().ToLower();

            AddDomainEvent(new SupplierUpdatedEvent(Id));
        }

        private void ValidateParameters(string name, string contactEmail)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Le nom du fournisseur est requis");

            if (string.IsNullOrWhiteSpace(contactEmail))
                throw new DomainException("L'email de contact est requis");

            if (!IsValidEmail(contactEmail))
                throw new DomainException("L'email de contact n'est pas valide");
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