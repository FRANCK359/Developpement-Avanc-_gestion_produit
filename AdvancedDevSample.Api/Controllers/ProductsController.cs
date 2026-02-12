using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
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
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService ??
                throw new ArgumentNullException(nameof(productService));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Récupère un produit par son ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            _logger.LogInformation("Requête GET pour le produit avec l'ID: {ProductId}", id);

            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Récupère tous les produits
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            _logger.LogInformation("Requête GET pour tous les produits");

            var products = await _productService.GetAllAsync();

            return Ok(products);
        }

        /// <summary>
        /// Récupère les produits par fournisseur
        /// </summary>
        [HttpGet("by-supplier/{supplierId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetBySupplier(Guid supplierId)
        {
            _logger.LogInformation("Requête GET pour les produits du fournisseur: {SupplierId}", supplierId);

            var products = await _productService.GetBySupplierAsync(supplierId);

            return Ok(products);
        }

        /// <summary>
        /// Récupère les produits actifs
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetActiveProducts()
        {
            _logger.LogInformation("Requête GET pour les produits actifs");

            var products = await _productService.GetActiveProductsAsync();

            return Ok(products);
        }

        /// <summary>
        /// Crée un nouveau produit
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto createDto)
        {
            _logger.LogInformation("Requête POST pour créer un nouveau produit: {ProductName}", createDto.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productService.CreateAsync(createDto);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        /// <summary>
        /// Met à jour un produit existant
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto updateDto)
        {
            _logger.LogInformation("Requête PUT pour mettre à jour le produit: {ProductId}", id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productService.UpdateAsync(id, updateDto);

            return Ok(product);
        }

        /// <summary>
        /// Change le prix d'un produit
        /// </summary>
        [HttpPatch("{id:guid}/price")]
        [ProducesResponseType(typeof(PriceChangeResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PriceChangeResponseDto>> ChangePrice(Guid id, [FromBody] decimal priceChange)
        {
            _logger.LogInformation("Requête PATCH pour changer le prix du produit: {ProductId} à {NewPrice}", id, priceChange);

            var product = await _productService.ChangePriceAsync(id, priceChange);

            var response = new PriceChangeResponseDto
            {
                ProductId = product.Id,
                NewPrice = product.Price,
                // Note: Pour obtenir l'ancien prix, il faudrait le stocker dans le service
                OldPrice = product.Price // Ceci serait l'ancien prix en réalité
            };

            return Ok(response);
        }

        /// <summary>
        /// Applique une remise sur un produit
        /// </summary>
        [HttpPatch("{id:guid}/discount")]
        [ProducesResponseType(typeof(DiscountResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DiscountResponseDto>> ApplyDiscount(Guid id, [FromBody] decimal discount)
        {
            _logger.LogInformation("Requête PATCH pour appliquer une remise de {Discount}% sur le produit: {ProductId}", discount * 100, id);

            var product = await _productService.ApplyDiscountAsync(id, discount);

            var response = new DiscountResponseDto
            {
                ProductId = product.Id,
                DiscountPercentage = discount * 100,
                NewPrice = product.Price,
                // Note: Pour obtenir l'ancien prix, il faudrait le stocker dans le service
                OldPrice = product.Price // Ceci serait l'ancien prix en réalité
            };

            return Ok(response);
        }

        /// <summary>
        /// Active un produit
        /// </summary>
        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductDto>> Activate(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour activer le produit: {ProductId}", id);

            var product = await _productService.ActivateAsync(id);

            return Ok(product);
        }

        /// <summary>
        /// Désactive un produit
        /// </summary>
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductDto>> Deactivate(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour désactiver le produit: {ProductId}", id);

            var product = await _productService.DeactivateAsync(id);

            return Ok(product);
        }

        /// <summary>
        /// Supprime un produit
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Requête DELETE pour le produit: {ProductId}", id);

            await _productService.DeleteAsync(id);

            return NoContent();
        }
    }
}