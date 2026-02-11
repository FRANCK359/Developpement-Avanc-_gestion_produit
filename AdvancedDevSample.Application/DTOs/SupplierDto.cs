using System;

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
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO pour la création d'un fournisseur
    /// </summary>
    public class CreateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour la mise à jour d'un fournisseur
    /// </summary>
    public class UpdateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
    }
}