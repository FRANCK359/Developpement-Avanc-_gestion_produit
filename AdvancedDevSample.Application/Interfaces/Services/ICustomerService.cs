using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;

namespace AdvancedDevSample.Application.Interfaces.Services
{
    /// <summary>
    /// Service d'application pour la gestion des clients
    /// </summary>
    public interface ICustomerService
    {
        Task<CustomerDto> GetByIdAsync(Guid id);
        Task<CustomerDto> GetByEmailAsync(string email);
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync();
        Task<CustomerDto> CreateAsync(CreateCustomerDto createDto);
        Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto updateDto);
        Task<CustomerDto> ActivateAsync(Guid id);
        Task<CustomerDto> DeactivateAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}