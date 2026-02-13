using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Exceptions;

namespace AdvancedDevSample.Application.Interfaces.Services
{
    /// <summary>
    /// Service d'application pour la gestion des produits
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Récupère un produit par son identifiant unique
        /// </summary>
        /// <param name="id">Identifiant unique du produit</param>
        /// <returns>Les informations du produit</returns>
        /// <exception cref="NotFoundException">Levée lorsque le produit n'existe pas</exception>
        Task<ProductDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Récupère tous les produits du catalogue
        /// </summary>
        /// <returns>Liste de tous les produits</returns>
        Task<IEnumerable<ProductDto>> GetAllAsync();

        /// <summary>
        /// Récupère tous les produits d'un fournisseur spécifique
        /// </summary>
        /// <param name="supplierId">Identifiant du fournisseur</param>
        /// <returns>Liste des produits du fournisseur</returns>
        Task<IEnumerable<ProductDto>> GetBySupplierAsync(Guid supplierId);

        /// <summary>
        /// Récupère uniquement les produits actifs
        /// </summary>
        /// <returns>Liste des produits actifs</returns>
        Task<IEnumerable<ProductDto>> GetActiveProductsAsync();

        /// <summary>
        /// Crée un nouveau produit dans le catalogue
        /// </summary>
        /// <param name="createDto">Données du produit à créer</param>
        /// <returns>Le produit créé avec son identifiant</returns>
        /// <exception cref="ValidationException">Levée lorsque les données sont invalides (prix négatif, nom vide)</exception>
        /// <exception cref="NotFoundException">Levée lorsque le fournisseur n'existe pas</exception>
        /// <exception cref="ConflictException">Levée lorsqu'un produit avec le même nom existe déjà</exception>
        Task<ProductDto> CreateAsync(CreateProductDto createDto);

        /// <summary>
        /// Met à jour les informations d'un produit existant
        /// </summary>
        /// <param name="id">Identifiant du produit à modifier</param>
        /// <param name="updateDto">Nouvelles données du produit</param>
        /// <returns>Le produit mis à jour</returns>
        /// <exception cref="NotFoundException">Levée lorsque le produit ou le fournisseur n'existe pas</exception>
        /// <exception cref="ValidationException">Levée lorsque les données sont invalides</exception>
        Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateDto);

        /// <summary>
        /// Modifie le prix d'un produit
        /// </summary>
        /// <param name="id">Identifiant du produit</param>
        /// <param name="newPrice">Nouveau prix du produit (doit être supérieur à 0)</param>
        /// <returns>Le produit avec le prix mis à jour</returns>
        /// <exception cref="NotFoundException">Levée lorsque le produit n'existe pas</exception>
        /// <exception cref="ValidationException">Levée lorsque le prix est négatif ou nul</exception>
        Task<ProductDto> ChangePriceAsync(Guid id, decimal newPrice);

        /// <summary>
        /// Applique une remise en pourcentage sur le prix d'un produit
        /// </summary>
        /// <param name="id">Identifiant du produit</param>
        /// <param name="discount">Pourcentage de remise (entre 0 et 1, ex: 0.15 pour 15%)</param>
        /// <returns>Le produit avec le prix réduit</returns>
        /// <exception cref="NotFoundException">Levée lorsque le produit n'existe pas</exception>
        /// <exception cref="ValidationException">Levée lorsque le pourcentage de remise est invalide (négatif ou supérieur à 100%)</exception>
        Task<ProductDto> ApplyDiscountAsync(Guid id, decimal discount);

        /// <summary>
        /// Active un produit désactivé pour le rendre disponible à la vente
        /// </summary>
        /// <param name="id">Identifiant du produit à activer</param>
        /// <returns>Le produit activé</returns>
        /// <exception cref="NotFoundException">Levée lorsque le produit n'existe pas</exception>
        Task<ProductDto> ActivateAsync(Guid id);

        /// <summary>
        /// Désactive un produit pour le retirer temporairement de la vente
        /// </summary>
        /// <param name="id">Identifiant du produit à désactiver</param>
        /// <returns>Le produit désactivé</returns>
        /// <exception cref="NotFoundException">Levée lorsque le produit n'existe pas</exception>
        Task<ProductDto> DeactivateAsync(Guid id);

        /// <summary>
        /// Supprime définitivement un produit du catalogue
        /// </summary>
        /// <param name="id">Identifiant du produit à supprimer</param>
        /// <exception cref="NotFoundException">Levée lorsque le produit n'existe pas</exception>
        /// <exception cref="ConflictException">Levée lorsque le produit est utilisé dans des commandes existantes</exception>
        Task DeleteAsync(Guid id);
    }
}