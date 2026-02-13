using System;
using System.ComponentModel.DataAnnotations;

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
        public string FormattedPrice => $"{Price:C}";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour la création d'un produit
    /// </summary>
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Le nom du produit est requis")]
        [MaxLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères")]
        [MinLength(3, ErrorMessage = "Le nom doit contenir au moins 3 caractères")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Le prix est requis")]
        [Range(0.01, 1000000, ErrorMessage = "Le prix doit être entre 0.01 et 1,000,000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Le fournisseur est requis")]
        public Guid SupplierId { get; set; }
    }

    /// <summary>
    /// DTO pour la mise à jour d'un produit
    /// </summary>
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Le nom du produit est requis")]
        [MaxLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères")]
        [MinLength(3, ErrorMessage = "Le nom doit contenir au moins 3 caractères")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Le prix est requis")]
        [Range(0.01, 1000000, ErrorMessage = "Le prix doit être entre 0.01 et 1,000,000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Le fournisseur est requis")]
        public Guid SupplierId { get; set; }
    }

    /// <summary>
    /// DTO pour le changement de prix d'un produit
    /// </summary>
    public class ChangePriceDto
    {
        [Required(ErrorMessage = "Le nouveau prix est requis")]
        [Range(0.01, 1000000, ErrorMessage = "Le prix doit être entre 0.01 et 1,000,000")]
        public decimal NewPrice { get; set; }
    }

    /// <summary>
    /// DTO pour appliquer une remise sur un produit
    /// </summary>
    public class ApplyDiscountDto
    {
        [Required(ErrorMessage = "Le pourcentage de remise est requis")]
        [Range(0.01, 100, ErrorMessage = "La remise doit être entre 0.01% et 100%")]
        public decimal DiscountPercentage { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse de changement de prix
    /// </summary>
    public class PriceChangeResponseDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public decimal PriceChange => NewPrice - OldPrice;
        public decimal PercentageChange => OldPrice > 0
            ? Math.Round((PriceChange / OldPrice) * 100, 2)
            : 0;
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO pour la réponse de remise appliquée
    /// </summary>
    public class DiscountResponseDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public decimal AmountSaved => OldPrice - NewPrice;
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}