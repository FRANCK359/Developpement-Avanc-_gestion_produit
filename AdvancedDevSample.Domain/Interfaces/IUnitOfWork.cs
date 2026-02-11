using System;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedDevSample.Domain.Interfaces
{
    /// <summary>
    /// Interface pour l'Unit of Work pattern
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Sauvegarde tous les changements
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Démarre une nouvelle transaction
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Valide la transaction courante
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Annule la transaction courante
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Repository pour les produits
        /// </summary>
        IProductRepository Products { get; }

        /// <summary>
        /// Repository pour les clients
        /// </summary>
        ICustomerRepository Customers { get; }

        /// <summary>
        /// Repository pour les fournisseurs
        /// </summary>
        ISupplierRepository Suppliers { get; }

        /// <summary>
        /// Repository pour les commandes
        /// </summary>
        IOrderRepository Orders { get; }
    }
}