using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Domain.Entities;

namespace AdvancedDevSample.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository de produits
    /// </summary>
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetBySupplierAsync(Guid supplierId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name);
        Task<int> SaveChangesAsync();
    }
}