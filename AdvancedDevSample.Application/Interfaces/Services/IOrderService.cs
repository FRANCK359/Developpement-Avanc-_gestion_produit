using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Domain.Exceptions;

namespace AdvancedDevSample.Application.Interfaces.Services
{
    /// <summary>
    /// Service d'application pour la gestion des commandes
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Récupère une commande par son identifiant unique
        /// </summary>
        /// <param name="id">Identifiant unique de la commande</param>
        /// <returns>Les informations de la commande</returns>
        /// <exception cref="NotFoundException">Levée lorsque la commande n'existe pas</exception>
        Task<OrderDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Récupère toutes les commandes du système
        /// </summary>
        /// <returns>Liste de toutes les commandes</returns>
        Task<IEnumerable<OrderDto>> GetAllAsync();

        /// <summary>
        /// Récupère toutes les commandes d'un client spécifique
        /// </summary>
        /// <param name="customerId">Identifiant du client</param>
        /// <returns>Liste des commandes du client</returns>
        Task<IEnumerable<OrderDto>> GetByCustomerAsync(Guid customerId);

        /// <summary>
        /// Récupère les commandes par statut
        /// </summary>
        /// <param name="status">Statut des commandes (ex: Pending, Confirmed, Completed, Cancelled)</param>
        /// <returns>Liste des commandes ayant ce statut</returns>
        /// <exception cref="ValidationException">Levée lorsque le statut est invalide</exception>
        Task<IEnumerable<OrderDto>> GetByStatusAsync(string status);

        /// <summary>
        /// Crée une nouvelle commande
        /// </summary>
        /// <param name="createDto">Données de la commande à créer</param>
        /// <returns>La commande créée avec son identifiant</returns>
        /// <exception cref="ValidationException">Levée lorsque les données sont invalides (client inexistant, produits invalides, quantités incorrectes)</exception>
        /// <exception cref="NotFoundException">Levée lorsque le client ou un produit n'existe pas</exception>
        Task<OrderDto> CreateAsync(CreateOrderDto createDto);

        /// <summary>
        /// Ajoute un produit à une commande existante
        /// </summary>
        /// <param name="orderId">Identifiant de la commande</param>
        /// <param name="addProductDto">Informations du produit à ajouter (ID et quantité)</param>
        /// <returns>La commande mise à jour</returns>
        /// <exception cref="NotFoundException">Levée lorsque la commande ou le produit n'existe pas</exception>
        /// <exception cref="ValidationException">Levée lorsque la quantité est invalide</exception>
        /// <exception cref="DomainException">Levée lorsque la commande ne peut pas être modifiée (statut incorrect)</exception>
        Task<OrderDto> AddProductAsync(Guid orderId, AddProductToOrderDto addProductDto);

        /// <summary>
        /// Retire un produit d'une commande existante
        /// </summary>
        /// <param name="orderId">Identifiant de la commande</param>
        /// <param name="productId">Identifiant du produit à retirer</param>
        /// <returns>La commande mise à jour</returns>
        /// <exception cref="NotFoundException">Levée lorsque la commande ou le produit n'existe pas dans la commande</exception>
        /// <exception cref="DomainException">Levée lorsque la commande ne peut pas être modifiée (statut incorrect)</exception>
        Task<OrderDto> RemoveProductAsync(Guid orderId, Guid productId);

        /// <summary>
        /// Confirme une commande en attente
        /// </summary>
        /// <param name="id">Identifiant de la commande</param>
        /// <returns>La commande confirmée</returns>
        /// <exception cref="NotFoundException">Levée lorsque la commande n'existe pas</exception>
        /// <exception cref="DomainException">Levée lorsque la commande ne peut pas être confirmée (déjà confirmée, annulée ou complétée)</exception>
        Task<OrderDto> ConfirmAsync(Guid id);

        /// <summary>
        /// Annule une commande
        /// </summary>
        /// <param name="id">Identifiant de la commande</param>
        /// <returns>La commande annulée</returns>
        /// <exception cref="NotFoundException">Levée lorsque la commande n'existe pas</exception>
        /// <exception cref="DomainException">Levée lorsque la commande ne peut pas être annulée (déjà complétée)</exception>
        Task<OrderDto> CancelAsync(Guid id);

        /// <summary>
        /// Marque une commande comme complétée
        /// </summary>
        /// <param name="id">Identifiant de la commande</param>
        /// <returns>La commande complétée</returns>
        /// <exception cref="NotFoundException">Levée lorsque la commande n'existe pas</exception>
        /// <exception cref="DomainException">Levée lorsque la commande ne peut pas être complétée (non confirmée ou annulée)</exception>
        Task<OrderDto> CompleteAsync(Guid id);

        /// <summary>
        /// Supprime définitivement une commande du système
        /// </summary>
        /// <param name="id">Identifiant de la commande à supprimer</param>
        /// <exception cref="NotFoundException">Levée lorsque la commande n'existe pas</exception>
        /// <exception cref="DomainException">Levée lorsque la commande ne peut pas être supprimée (commande confirmée ou complétée)</exception>
        Task DeleteAsync(Guid id);
    }
}