using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Domain.Entities;

namespace AdvancedDevSample.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository de fournisseurs
    /// </summary>
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByIdAsync(Guid id);
        Task<Supplier?> GetByNameAsync(string name);
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<IEnumerable<Supplier>> GetActiveSuppliersAsync();
        Task<Supplier> AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name);
        Task<int> SaveChangesAsync();
    }
}