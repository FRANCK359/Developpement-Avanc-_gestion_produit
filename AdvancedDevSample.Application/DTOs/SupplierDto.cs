using System;
using System.ComponentModel.DataAnnotations;

namespace AdvancedDevSample.Application.DTOs
{
    /// <summary>
    /// DTO pour la représentation d'un fournisseur
    /// </summary>
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ProductCount { get; set; }
    }

    /// <summary>
    /// DTO de base pour les opérations sur les fournisseurs
    /// </summary>
    public abstract class SupplierBaseDto
    {
        [Required(ErrorMessage = "Le nom du fournisseur est requis")]
        [MaxLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères")]
        [MinLength(2, ErrorMessage = "Le nom doit contenir au moins 2 caractères")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email de contact est requis")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide")]
        [MaxLength(256, ErrorMessage = "L'email ne peut pas dépasser 256 caractères")]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide")]
        [MaxLength(20, ErrorMessage = "Le numéro de téléphone ne peut pas dépasser 20 caractères")]
        public string? Phone { get; set; }

        [MaxLength(500, ErrorMessage = "L'adresse ne peut pas dépasser 500 caractères")]
        public string? Address { get; set; }
    }

    /// <summary>
    /// DTO pour la création d'un fournisseur
    /// </summary>
    public class CreateSupplierDto : SupplierBaseDto
    {
    }

    /// <summary>
    /// DTO pour la mise à jour d'un fournisseur
    /// </summary>
    public class UpdateSupplierDto : SupplierBaseDto
    {
    }
}