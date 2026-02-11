using System;
using AdvancedDevSample.Domain.Common;
using AdvancedDevSample.Domain.Enums;
using AdvancedDevSample.Domain.Events;
using AdvancedDevSample.Domain.Exceptions;

namespace AdvancedDevSample.Domain.Entities
{
    /// <summary>
    /// Représente un produit dans le domaine
    /// </summary>
    public class Product : BaseEntity
    {
        private Product()
        {
            // Constructeur privé pour EF Core
            Name = string.Empty;
            Description = string.Empty;
        }

        /// <summary>
        /// Initialise une nouvelle instance de Product
        /// </summary>
        public Product(string name, string description, decimal initialPrice, Guid supplierId)
        {
            ValidateParameters(name, initialPrice, supplierId);

            Id = Guid.NewGuid();
            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Price = initialPrice;
            SupplierId = supplierId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new ProductCreatedEvent(this));
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid SupplierId { get; private set; }

        /// <summary>
        /// Change le prix du produit
        /// </summary>
        /// <param name="newPrice">Nouveau prix</param>
        /// <exception cref="DomainException">Si le prix est négatif</exception>
        public void ChangePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new DomainException("Le prix ne peut pas être négatif");

            if (Price != newPrice)
            {
                var oldPrice = Price;
                Price = newPrice;

                // Événement de domaine
                AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, newPrice));
            }
        }

        /// <summary>
        /// Applique une remise sur le prix
        /// </summary>
        /// <param name="discount">Pourcentage de remise (ex: 0.1 pour 10%)</param>
        /// <exception cref="DomainException">Si la remise n'est pas valide</exception>
        public void ApplyDiscount(decimal discount)
        {
            if (discount < 0 || discount > 1)
                throw new DomainException("La remise doit être entre 0 et 1 (0-100%)");

            if (discount > 0)
            {
                var oldPrice = Price;
                Price = Price * (1 - discount);

                // Événement de domaine
                AddDomainEvent(new ProductDiscountAppliedEvent(Id, discount, oldPrice, Price));
            }
        }

        /// <summary>
        /// Active le produit
        /// </summary>
        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                AddDomainEvent(new ProductActivatedEvent(Id));
            }
        }

        /// <summary>
        /// Désactive le produit
        /// </summary>
        public void Desactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                AddDomainEvent(new ProductDeactivatedEvent(Id));
            }
        }

        private void ValidateParameters(string name, decimal price, Guid supplierId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Le nom du produit est requis");

            if (price < 0)
                throw new DomainException("Le prix ne peut pas être négatif");

            if (supplierId == Guid.Empty)
                throw new DomainException("L'ID du fournisseur est requis");
        }
    }
}