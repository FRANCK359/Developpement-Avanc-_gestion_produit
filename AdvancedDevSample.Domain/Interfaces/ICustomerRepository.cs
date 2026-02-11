using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Domain.Entities;

namespace AdvancedDevSample.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository de clients
    /// </summary>
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();
        Task<Customer> AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<int> SaveChangesAsync();
    }
}