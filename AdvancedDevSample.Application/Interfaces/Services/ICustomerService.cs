using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Exceptions;

namespace AdvancedDevSample.Application.Interfaces.Services
{
    /// <summary>
    /// Service d'application pour la gestion des clients
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Récupère un client par son identifiant unique
        /// </summary>
        /// <param name="id">Identifiant unique du client</param>
        /// <returns>Les informations du client</returns>
        /// <exception cref="NotFoundException">Levée lorsque le client n'existe pas</exception>
        Task<CustomerDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Récupère un client par son adresse email
        /// </summary>
        /// <param name="email">Adresse email du client</param>
        /// <returns>Les informations du client</returns>
        /// <exception cref="NotFoundException">Levée lorsque le client n'existe pas</exception>
        Task<CustomerDto> GetByEmailAsync(string email);

        /// <summary>
        /// Récupère tous les clients du système
        /// </summary>
        /// <returns>Liste de tous les clients</returns>
        Task<IEnumerable<CustomerDto>> GetAllAsync();

        /// <summary>
        /// Récupère uniquement les clients actifs
        /// </summary>
        /// <returns>Liste des clients actifs</returns>
        Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync();

        /// <summary>
        /// Crée un nouveau client
        /// </summary>
        /// <param name="createDto">Données du client à créer</param>
        /// <returns>Le client créé avec son identifiant</returns>
        /// <exception cref="ValidationException">Levée lorsque les données sont invalides</exception>
        /// <exception cref="ConflictException">Levée lorsque l'email existe déjà</exception>
        Task<CustomerDto> CreateAsync(CreateCustomerDto createDto);

        /// <summary>
        /// Met à jour les informations d'un client existant
        /// </summary>
        /// <param name="id">Identifiant du client à modifier</param>
        /// <param name="updateDto">Nouvelles données du client</param>
        /// <returns>Le client mis à jour</returns>
        /// <exception cref="NotFoundException">Levée lorsque le client n'existe pas</exception>
        /// <exception cref="ValidationException">Levée lorsque les données sont invalides</exception>
        /// <exception cref="ConflictException">Levée lorsque le nouvel email existe déjà</exception>
        Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto updateDto);

        /// <summary>
        /// Active un client désactivé
        /// </summary>
        /// <param name="id">Identifiant du client à activer</param>
        /// <returns>Le client activé</returns>
        /// <exception cref="NotFoundException">Levée lorsque le client n'existe pas</exception>
        Task<CustomerDto> ActivateAsync(Guid id);

        /// <summary>
        /// Désactive un client actif
        /// </summary>
        /// <param name="id">Identifiant du client à désactiver</param>
        /// <returns>Le client désactivé</returns>
        /// <exception cref="NotFoundException">Levée lorsque le client n'existe pas</exception>
        Task<CustomerDto> DeactivateAsync(Guid id);

        /// <summary>
        /// Supprime définitivement un client du système
        /// </summary>
        /// <param name="id">Identifiant du client à supprimer</param>
        /// <exception cref="NotFoundException">Levée lorsque le client n'existe pas</exception>
        Task DeleteAsync(Guid id);
    }
}