using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Domain.Interfaces;
using AdvancedDevSample.Domain.Entities;

namespace AdvancedDevSample.Application.Services
{
    /// <summary>
    /// Implémentation du service de gestion des produits
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            ISupplierRepository supplierRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository ??
                throw new ArgumentNullException(nameof(productRepository));
            _supplierRepository = supplierRepository ??
                throw new ArgumentNullException(nameof(supplierRepository));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductDto> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Récupération du produit avec l'ID {ProductId}", id);

            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                throw new NotFoundException($"Produit avec l'ID {id} non trouvé");

            return MapToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            _logger.LogInformation("Récupération de tous les produits");

            var products = await _productRepository.GetAllAsync();

            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetBySupplierAsync(Guid supplierId)
        {
            _logger.LogInformation("Récupération des produits pour le fournisseur {SupplierId}", supplierId);

            var products = await _productRepository.GetBySupplierAsync(supplierId);

            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
        {
            _logger.LogInformation("Récupération des produits actifs");

            var products = await _productRepository.GetActiveProductsAsync();

            return products.Select(MapToDto);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto createDto)
        {
            _logger.LogInformation("Création d'un nouveau produit: {ProductName}", createDto.Name);

            // Vérifier si le fournisseur existe
            var supplier = await _supplierRepository.GetByIdAsync(createDto.SupplierId);
            if (supplier == null)
                throw new ValidationException($"Fournisseur avec l'ID {createDto.SupplierId} non trouvé");

            if (!supplier.IsActive)
                throw new ValidationException("Le fournisseur n'est pas actif");

            var product = new Product(
                createDto.Name,
                createDto.Description ?? string.Empty,
                createDto.Price,
                createDto.SupplierId);

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Produit créé avec succès: {ProductId}", product.Id);

            return MapToDto(product);
        }

        public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateDto)
        {
            _logger.LogInformation("Mise à jour du produit {ProductId}", id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Produit avec l'ID {id} non trouvé");

            product.ChangePrice(updateDto.Price);

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Produit mis à jour avec succès: {ProductId}", id);

            return MapToDto(product);
        }

        public async Task<ProductDto> ChangePriceAsync(Guid id, decimal newPrice)
        {
            _logger.LogInformation("Changement de prix pour le produit {ProductId}: {NewPrice}", id, newPrice);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Produit avec l'ID {id} non trouvé");

            product.ChangePrice(newPrice);

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Prix changé avec succès pour le produit {ProductId}", id);

            return MapToDto(product);
        }

        public async Task<ProductDto> ApplyDiscountAsync(Guid id, decimal discount)
        {
            _logger.LogInformation("Application d'une remise de {Discount}% sur le produit {ProductId}", discount * 100, id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Produit avec l'ID {id} non trouvé");

            product.ApplyDiscount(discount);

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Remise appliquée avec succès sur le produit {ProductId}", id);

            return MapToDto(product);
        }

        public async Task<ProductDto> ActivateAsync(Guid id)
        {
            _logger.LogInformation("Activation du produit {ProductId}", id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Produit avec l'ID {id} non trouvé");

            product.Activate();

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Produit activé avec succès: {ProductId}", id);

            return MapToDto(product);
        }

        public async Task<ProductDto> DeactivateAsync(Guid id)
        {
            _logger.LogInformation("Désactivation du produit {ProductId}", id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Produit avec l'ID {id} non trouvé");

            product.Desactivate();

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Produit désactivé avec succès: {ProductId}", id);

            return MapToDto(product);
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Suppression du produit {ProductId}", id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Produit avec l'ID {id} non trouvé");

            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Produit supprimé avec succès: {ProductId}", id);
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                SupplierId = product.SupplierId
            };
        }
    }
}