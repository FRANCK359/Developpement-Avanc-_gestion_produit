using System;
using AdvancedDevSample.Domain.Common;

namespace AdvancedDevSample.Domain.Events
{
    /// <summary>
    /// Événement déclenché lors de la création d'un produit
    /// </summary>
    public class ProductCreatedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string ProductName { get; }
        public decimal Price { get; }
        public Guid SupplierId { get; }

        public ProductCreatedEvent(AdvancedDevSample.Domain.Entities.Product product)
        {
            ProductId = product.Id;
            ProductName = product.Name;
            Price = product.Price;
            SupplierId = product.SupplierId;
        }
    }

    /// <summary>
    /// Événement déclenché lors du changement de prix d'un produit
    /// </summary>
    public class ProductPriceChangedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }

        public ProductPriceChangedEvent(Guid productId, decimal oldPrice, decimal newPrice)
        {
            ProductId = productId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
        }
    }

    /// <summary>
    /// Événement déclenché lors de l'application d'une remise
    /// </summary>
    public class ProductDiscountAppliedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public decimal DiscountPercentage { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }

        public ProductDiscountAppliedEvent(Guid productId, decimal discount, decimal oldPrice, decimal newPrice)
        {
            ProductId = productId;
            DiscountPercentage = discount * 100;
            OldPrice = oldPrice;
            NewPrice = newPrice;
        }
    }

    /// <summary>
    /// Événement déclenché lors de l'activation d'un produit
    /// </summary>
    public class ProductActivatedEvent : DomainEvent
    {
        public Guid ProductId { get; }

        public ProductActivatedEvent(Guid productId)
        {
            ProductId = productId;
        }
    }

    /// <summary>
    /// Événement déclenché lors de la désactivation d'un produit
    /// </summary>
    public class ProductDeactivatedEvent : DomainEvent
    {
        public Guid ProductId { get; }

        public ProductDeactivatedEvent(Guid productId)
        {
            ProductId = productId;
        }
    }
}