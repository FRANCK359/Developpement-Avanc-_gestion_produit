// ProductsController.cs - VERSION REFACTORISÉE
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
<<<<<<< Updated upstream
using Microsoft.AspNetCore.Authorization;
=======
>>>>>>> Stashed changes
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedDevSample.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des produits
    /// </summary>
    public class ProductsController : BaseApiController<ProductsController>
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
            : base(logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<ProductDto>> GetById(Guid id) =>
            ExecuteAsync(() => _productService.GetByIdAsync(id),
                "Requête GET pour le produit avec l'ID: {ProductId}", id);

<<<<<<< Updated upstream
            var product = await _productService.GetByIdAsync(id);

            return Ok(product);
        }

        /// <summary>
        /// Récupère tous les produits
        /// </summary>
=======
>>>>>>> Stashed changes
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<ProductDto>>> GetAll() =>
            ExecuteAsync(() => _productService.GetAllAsync(),
                "Requête GET pour tous les produits");

        [HttpGet("by-supplier/{supplierId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<ProductDto>>> GetBySupplier(Guid supplierId) =>
            ExecuteAsync(() => _productService.GetBySupplierAsync(supplierId),
                "Requête GET pour les produits du fournisseur: {SupplierId}", supplierId);

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<ProductDto>>> GetActiveProducts() =>
            ExecuteAsync(() => _productService.GetActiveProductsAsync(),
                "Requête GET pour les produits actifs");

        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto createDto) =>
            ExecuteCreationAsync(
                () => _productService.CreateAsync(createDto),
                product => new { id = product.Id },
                nameof(GetById),
                "Requête POST pour créer un nouveau produit: {ProductName}", createDto.Name);

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto updateDto) =>
            ExecuteAsync(() => _productService.UpdateAsync(id, updateDto),
                "Requête PUT pour mettre à jour le produit: {ProductId}", id);

        [HttpPatch("{id:guid}/price")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductDto>> ChangePrice(Guid id, [FromBody] decimal newPrice)
        {
            Logger.LogInformation("Requête PATCH pour changer le prix du produit: {ProductId} à {NewPrice}", id, newPrice);
            var product = await _productService.ChangePriceAsync(id, newPrice);
            return Ok(product);
        }

        [HttpPatch("{id:guid}/discount")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductDto>> ApplyDiscount(Guid id, [FromBody] decimal discount)
        {
            Logger.LogInformation("Requête PATCH pour appliquer une remise de {Discount}% sur le produit: {ProductId}",
                discount * 100, id);
            var product = await _productService.ApplyDiscountAsync(id, discount);
            return Ok(product);
        }

        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public Task<ActionResult<ProductDto>> Activate(Guid id) =>
            ExecuteAsync(() => _productService.ActivateAsync(id),
                "Requête PATCH pour activer le produit: {ProductId}", id);

        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public Task<ActionResult<ProductDto>> Deactivate(Guid id) =>
            ExecuteAsync(() => _productService.DeactivateAsync(id),
                "Requête PATCH pour désactiver le produit: {ProductId}", id);

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Delete(Guid id) =>
            ExecuteDeleteAsync(() => _productService.DeleteAsync(id),
                "Requête DELETE pour le produit: {ProductId}", id);
    }
}