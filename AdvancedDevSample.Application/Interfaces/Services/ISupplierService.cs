using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;

namespace AdvancedDevSample.Application.Interfaces.Services
{
    /// <summary>
    /// Service d'application pour la gestion des fournisseurs
    /// </summary>
    public interface ISupplierService
    {
        Task<SupplierDto> GetByIdAsync(Guid id);
        Task<SupplierDto> GetByNameAsync(string name);
        Task<IEnumerable<SupplierDto>> GetAllAsync();
        Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync();
        Task<SupplierDto> CreateAsync(CreateSupplierDto createDto);
        Task<SupplierDto> UpdateAsync(Guid id, UpdateSupplierDto updateDto);
        Task<SupplierDto> ActivateAsync(Guid id);
        Task<SupplierDto> DeactivateAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}