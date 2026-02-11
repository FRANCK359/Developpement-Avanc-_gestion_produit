using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AdvancedDevSample.Application.Services
{
    /// <summary>
    /// Implémentation du service de gestion des fournisseurs
    /// </summary>
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(
            ISupplierRepository supplierRepository,
            ILogger<SupplierService> logger)
        {
            _supplierRepository = supplierRepository ??
                throw new ArgumentNullException(nameof(supplierRepository));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SupplierDto> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Récupération du fournisseur avec l'ID {SupplierId}", id);

            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
                throw new NotFoundException($"Fournisseur avec l'ID {id} non trouvé");

            return MapToDto(supplier);
        }

        public async Task<SupplierDto> GetByNameAsync(string name)
        {
            _logger.LogInformation("Récupération du fournisseur avec le nom {Name}", name);

            var supplier = await _supplierRepository.GetByNameAsync(name);

            if (supplier == null)
                throw new NotFoundException($"Fournisseur avec le nom {name} non trouvé");

            return MapToDto(supplier);
        }

        public async Task<IEnumerable<SupplierDto>> GetAllAsync()
        {
            _logger.LogInformation("Récupération de tous les fournisseurs");

            var suppliers = await _supplierRepository.GetAllAsync();

            return suppliers.Select(MapToDto);
        }

        public async Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync()
        {
            _logger.LogInformation("Récupération des fournisseurs actifs");

            var suppliers = await _supplierRepository.GetActiveSuppliersAsync();

            return suppliers.Select(MapToDto);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto createDto)
        {
            _logger.LogInformation("Création d'un nouveau fournisseur: {Name}", createDto.Name);

            // Vérifier si le nom existe déjà
            var existingSupplier = await _supplierRepository.GetByNameAsync(createDto.Name);
            if (existingSupplier != null)
                throw new ValidationException($"Un fournisseur avec le nom {createDto.Name} existe déjà");

            var supplier = new Supplier(
                createDto.Name,
                createDto.ContactEmail);

            await _supplierRepository.AddAsync(supplier);
            await _supplierRepository.SaveChangesAsync();

            _logger.LogInformation("Fournisseur créé avec succès: {SupplierId}", supplier.Id);

            return MapToDto(supplier);
        }

        public async Task<SupplierDto> UpdateAsync(Guid id, UpdateSupplierDto updateDto)
        {
            _logger.LogInformation("Mise à jour du fournisseur {SupplierId}", id);

            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                throw new NotFoundException($"Fournisseur avec l'ID {id} non trouvé");

            // Vérifier si le nouveau nom existe déjà pour un autre fournisseur
            if (supplier.Name != updateDto.Name)
            {
                var existingSupplier = await _supplierRepository.GetByNameAsync(updateDto.Name);
                if (existingSupplier != null && existingSupplier.Id != id)
                    throw new ValidationException($"Un fournisseur avec le nom {updateDto.Name} existe déjà");
            }

            supplier.Update(
                updateDto.Name,
                updateDto.ContactEmail);

            await _supplierRepository.UpdateAsync(supplier);
            await _supplierRepository.SaveChangesAsync();

            _logger.LogInformation("Fournisseur mis à jour avec succès: {SupplierId}", id);

            return MapToDto(supplier);
        }

        public async Task<SupplierDto> ActivateAsync(Guid id)
        {
            _logger.LogInformation("Activation du fournisseur {SupplierId}", id);

            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                throw new NotFoundException($"Fournisseur avec l'ID {id} non trouvé");

            supplier.Activate();

            await _supplierRepository.UpdateAsync(supplier);
            await _supplierRepository.SaveChangesAsync();

            _logger.LogInformation("Fournisseur activé avec succès: {SupplierId}", id);

            return MapToDto(supplier);
        }

        public async Task<SupplierDto> DeactivateAsync(Guid id)
        {
            _logger.LogInformation("Désactivation du fournisseur {SupplierId}", id);

            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                throw new NotFoundException($"Fournisseur avec l'ID {id} non trouvé");

            supplier.Desactivate();

            await _supplierRepository.UpdateAsync(supplier);
            await _supplierRepository.SaveChangesAsync();

            _logger.LogInformation("Fournisseur désactivé avec succès: {SupplierId}", id);

            return MapToDto(supplier);
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Suppression du fournisseur {SupplierId}", id);

            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                throw new NotFoundException($"Fournisseur avec l'ID {id} non trouvé");

            await _supplierRepository.DeleteAsync(id);
            await _supplierRepository.SaveChangesAsync();

            _logger.LogInformation("Fournisseur supprimé avec succès: {SupplierId}", id);
        }

        private SupplierDto MapToDto(Supplier supplier)
        {
            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactEmail = supplier.ContactEmail,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt
            };
        }
    }
}