using AdvancedDevSample.Domain.Common;
using AdvancedDevSample.Domain.Exceptions;
using System;

namespace AdvancedDevSample.Domain.Entities
{
    /// <summary>
    /// Représente un élément de commande
    /// </summary>
    public class OrderItem
    {
        private OrderItem() { } // Constructeur privé pour EF Core

        /// <summary>
        /// Initialise une nouvelle instance de OrderItem
        /// </summary>
        public OrderItem(Guid productId, int quantity, decimal unitPrice)
        {
            ValidateParameters(productId, quantity, unitPrice);

            Id = Guid.NewGuid();
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        /// <summary>
        /// Calcule le sous-total pour cet élément
        /// </summary>
        public decimal GetSubTotal() => UnitPrice * Quantity;

        /// <summary>
        /// Met à jour la quantité
        /// </summary>
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new DomainException("La quantité doit être supérieure à 0");

            Quantity = newQuantity;
        }

        private void ValidateParameters(Guid productId, int quantity, decimal unitPrice)
        {
            if (productId == Guid.Empty)
                throw new DomainException("L'ID du produit est requis");

            if (quantity <= 0)
                throw new DomainException("La quantité doit être supérieure à 0");

            if (unitPrice < 0)
                throw new DomainException("Le prix unitaire ne peut pas être négatif");
        }
    }
}