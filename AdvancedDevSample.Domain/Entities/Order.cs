using AdvancedDevSample.Domain.Common;
using AdvancedDevSample.Domain.Enums;
using AdvancedDevSample.Domain.Events;
using AdvancedDevSample.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedDevSample.Domain.Entities
{
    /// <summary>
    /// Représente une commande dans le domaine
    /// </summary>
    public class Order : BaseEntity
    {
        private readonly List<OrderItem> _orderItems = new();

        private Order() { } // Constructeur privé pour EF Core

        /// <summary>
        /// Initialise une nouvelle instance de Order
        /// </summary>
        public Order(Guid customerId)
        {
            if (customerId == Guid.Empty)
                throw new DomainException("L'ID du client est requis");

            Id = Guid.NewGuid();
            CustomerId = customerId;
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Pending;
            TotalAmount = 0;
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new OrderCreatedEvent(this));
        }

        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public Guid CustomerId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        /// <summary>
        /// Ajoute un produit à la commande
        /// </summary>
        public void AddProduct(Product product, int quantity)
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("Seules les commandes en attente peuvent être modifiées");

            if (product == null)
                throw new DomainException("Le produit est requis");

            if (!product.IsActive)
                throw new DomainException("Le produit n'est pas actif");

            if (quantity <= 0)
                throw new DomainException("La quantité doit être supérieure à 0");

            var existingItem = _orderItems.FirstOrDefault(item => item.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                _orderItems.Add(new OrderItem(product.Id, quantity, product.Price));
            }

            CalculateTotal();
            AddDomainEvent(new ProductAddedToOrderEvent(Id, product.Id, quantity));
        }

        /// <summary>
        /// Supprime un produit de la commande
        /// </summary>
        public void RemoveProduct(Guid productId)
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("Seules les commandes en attente peuvent être modifiées");

            var item = _orderItems.FirstOrDefault(item => item.ProductId == productId);

            if (item == null)
                throw new DomainException("Produit non trouvé dans la commande");

            _orderItems.Remove(item);
            CalculateTotal();
            AddDomainEvent(new ProductRemovedFromOrderEvent(Id, productId));
        }

        /// <summary>
        /// Confirme la commande
        /// </summary>
        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("Seules les commandes en attente peuvent être confirmées");

            if (!_orderItems.Any())
                throw new DomainException("La commande doit contenir au moins un produit");

            Status = OrderStatus.Confirmed;
            CalculateTotal();
            AddDomainEvent(new OrderConfirmedEvent(Id));
        }

        /// <summary>
        /// Annule la commande
        /// </summary>
        public void Cancel()
        {
            if (Status == OrderStatus.Cancelled || Status == OrderStatus.Completed)
                throw new DomainException("La commande ne peut pas être annulée dans son état actuel");

            Status = OrderStatus.Cancelled;
            AddDomainEvent(new OrderCancelledEvent(Id));
        }

        /// <summary>
        /// Marque la commande comme complétée
        /// </summary>
        public void Complete()
        {
            if (Status != OrderStatus.Confirmed)
                throw new DomainException("Seules les commandes confirmées peuvent être complétées");

            Status = OrderStatus.Completed;
            AddDomainEvent(new OrderCompletedEvent(Id));
        }

        /// <summary>
        /// Calcule le montant total de la commande
        /// </summary>
        public void CalculateTotal()
        {
            TotalAmount = _orderItems.Sum(item => item.UnitPrice * item.Quantity);

            if (Status == OrderStatus.Pending)
            {
                AddDomainEvent(new OrderTotalCalculatedEvent(Id, TotalAmount));
            }
        }
    }
}