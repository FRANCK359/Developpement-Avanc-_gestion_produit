using System;

namespace AdvancedDevSample.Application.DTOs
{
    /// <summary>
    /// DTO pour la représentation d'un produit
    /// </summary>
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid SupplierId { get; set; }
    }

    /// <summary>
    /// DTO pour la création d'un produit
    /// </summary>
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid SupplierId { get; set; }
    }

    /// <summary>
    /// DTO pour la mise à jour d'un produit
    /// </summary>
    public class UpdateProductDto
    {
        public decimal Price { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse de prix changé
    /// </summary>
    public class PriceChangeResponseDto
    {
        public Guid ProductId { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse de remise appliquée
    /// </summary>
    public class DiscountResponseDto
    {
        public Guid ProductId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
    }
}