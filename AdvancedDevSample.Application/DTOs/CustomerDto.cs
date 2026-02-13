using System;
using System.ComponentModel.DataAnnotations;

namespace AdvancedDevSample.Application.DTOs
{
    /// <summary>
    /// DTO pour la représentation d'un client
    /// </summary>
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO pour la création d'un client
    /// </summary>
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "Le prénom est requis")]
        [MaxLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères")]
        [MinLength(2, ErrorMessage = "Le prénom doit contenir au moins 2 caractères")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        [MinLength(2, ErrorMessage = "Le nom doit contenir au moins 2 caractères")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide")]
        [MaxLength(256, ErrorMessage = "L'email ne peut pas dépasser 256 caractères")]
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour la mise à jour d'un client
    /// </summary>
    public class UpdateCustomerDto
    {
        [Required(ErrorMessage = "Le prénom est requis")]
        [MaxLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères")]
        [MinLength(2, ErrorMessage = "Le prénom doit contenir au moins 2 caractères")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        [MinLength(2, ErrorMessage = "Le nom doit contenir au moins 2 caractères")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide")]
        [MaxLength(256, ErrorMessage = "L'email ne peut pas dépasser 256 caractères")]
        public string Email { get; set; } = string.Empty;
    }
}