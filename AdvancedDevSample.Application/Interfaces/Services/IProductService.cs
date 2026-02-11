using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;

namespace AdvancedDevSample.Application.Interfaces.Services
{
    /// <summary>
    /// Service d'application pour la gestion des produits
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Récupère un produit par son ID
        /// </summary>
        Task<ProductDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Récupère tous les produits
        /// </summary>
        Task<IEnumerable<ProductDto>> GetAllAsync();

        /// <summary>
        /// Récupère les produits par fournisseur
        /// </summary>
        Task<IEnumerable<ProductDto>> GetBySupplierAsync(Guid supplierId);

        /// <summary>
        /// Récupère les produits actifs
        /// </summary>
        Task<IEnumerable<ProductDto>> GetActiveProductsAsync();

        /// <summary>
        /// Crée un nouveau produit
        /// </summary>
        Task<ProductDto> CreateAsync(CreateProductDto createDto);

        /// <summary>
        /// Met à jour un produit existant
        /// </summary>
        Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateDto);

        /// <summary>
        /// Change le prix d'un produit
        /// </summary>
        Task<ProductDto> ChangePriceAsync(Guid id, decimal newPrice);

        /// <summary>
        /// Applique une remise sur un produit
        /// </summary>
        Task<ProductDto> ApplyDiscountAsync(Guid id, decimal discount);

        /// <summary>
        /// Active un produit
        /// </summary>
        Task<ProductDto> ActivateAsync(Guid id);

        /// <summary>
        /// Désactive un produit
        /// </summary>
        Task<ProductDto> DeactivateAsync(Guid id);

        /// <summary>
        /// Supprime un produit
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}