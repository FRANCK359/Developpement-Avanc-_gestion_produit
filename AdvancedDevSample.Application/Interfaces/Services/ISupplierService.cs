using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Exceptions;

namespace AdvancedDevSample.Application.Interfaces.Services
{
    /// <summary>
    /// Service d'application pour la gestion des fournisseurs
    /// </summary>
    public interface ISupplierService
    {
        /// <summary>
        /// Récupère un fournisseur par son identifiant unique
        /// </summary>
        /// <param name="id">Identifiant unique du fournisseur</param>
        /// <returns>Les informations du fournisseur</returns>
        /// <exception cref="NotFoundException">Levée lorsque le fournisseur n'existe pas</exception>
        Task<SupplierDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Récupère un fournisseur par son nom
        /// </summary>
        /// <param name="name">Nom du fournisseur</param>
        /// <returns>Les informations du fournisseur</returns>
        /// <exception cref="NotFoundException">Levée lorsque le fournisseur n'existe pas</exception>
        Task<SupplierDto> GetByNameAsync(string name);

        /// <summary>
        /// Récupère tous les fournisseurs du système
        /// </summary>
        /// <returns>Liste de tous les fournisseurs</returns>
        Task<IEnumerable<SupplierDto>> GetAllAsync();

        /// <summary>
        /// Récupère uniquement les fournisseurs actifs
        /// </summary>
        /// <returns>Liste des fournisseurs actifs</returns>
        Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync();

        /// <summary>
        /// Crée un nouveau fournisseur
        /// </summary>
        /// <param name="createDto">Données du fournisseur à créer</param>
        /// <returns>Le fournisseur créé avec son identifiant</returns>
        /// <exception cref="ValidationException">Levée lorsque les données sont invalides (email invalide, nom vide)</exception>
        /// <exception cref="ConflictException">Levée lorsqu'un fournisseur avec le même nom ou email existe déjà</exception>
        Task<SupplierDto> CreateAsync(CreateSupplierDto createDto);

        /// <summary>
        /// Met à jour les informations d'un fournisseur existant
        /// </summary>
        /// <param name="id">Identifiant du fournisseur à modifier</param>
        /// <param name="updateDto">Nouvelles données du fournisseur</param>
        /// <returns>Le fournisseur mis à jour</returns>
        /// <exception cref="NotFoundException">Levée lorsque le fournisseur n'existe pas</exception>
        /// <exception cref="ValidationException">Levée lorsque les données sont invalides</exception>
        /// <exception cref="ConflictException">Levée lorsque le nouveau nom ou email existe déjà</exception>
        Task<SupplierDto> UpdateAsync(Guid id, UpdateSupplierDto updateDto);

        /// <summary>
        /// Active un fournisseur désactivé
        /// </summary>
        /// <param name="id">Identifiant du fournisseur à activer</param>
        /// <returns>Le fournisseur activé</returns>
        /// <exception cref="NotFoundException">Levée lorsque le fournisseur n'existe pas</exception>
        Task<SupplierDto> ActivateAsync(Guid id);

        /// <summary>
        /// Désactive un fournisseur actif
        /// </summary>
        /// <param name="id">Identifiant du fournisseur à désactiver</param>
        /// <returns>Le fournisseur désactivé</returns>
        /// <exception cref="NotFoundException">Levée lorsque le fournisseur n'existe pas</exception>
        Task<SupplierDto> DeactivateAsync(Guid id);

        /// <summary>
        /// Supprime définitivement un fournisseur du système
        /// </summary>
        /// <param name="id">Identifiant du fournisseur à supprimer</param>
        /// <exception cref="NotFoundException">Levée lorsque le fournisseur n'existe pas</exception>
        /// <exception cref="ConflictException">Levée lorsque le fournisseur a des produits associés</exception>
        Task DeleteAsync(Guid id);
    }
}