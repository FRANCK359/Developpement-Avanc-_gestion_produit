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
    [Authorize] // Auth obligatoire pour toutes les actions
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            _logger.LogInformation("Requête GET pour tous les produits");
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("by-supplier/{supplierId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetBySupplier(Guid supplierId)
        {
            _logger.LogInformation("Requête GET pour les produits du fournisseur: {SupplierId}", supplierId);
            var products = await _productService.GetBySupplierAsync(supplierId);
            return Ok(products);
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetActiveProducts()
        {
            _logger.LogInformation("Requête GET pour les produits actifs");
            var products = await _productService.GetActiveProductsAsync();
            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto createDto)
        {
            // CORRECTION: Vérification de null AVANT toute utilisation
            if (createDto == null)
            {
                return BadRequest("Les données du produit sont requises.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // CORRECTION: Utilisation de l'opérateur conditionnel avec vérification de null
            _logger.LogInformation(
                "Requête POST pour créer un produit: {ProductName}",
                createDto.Name ?? "Nom non spécifié");

            try
            {
                var product = await _productService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du produit");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto updateDto)
        {
            // CORRECTION: Vérification de null
            if (updateDto == null)
            {
                return BadRequest("Les données de mise à jour sont requises.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Requête PUT pour mettre à jour le produit: {ProductId}", id);

            try
            {
                var product = await _productService.UpdateAsync(id, updateDto);
                return Ok(product);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du produit {ProductId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/price")]
        [ProducesResponseType(typeof(PriceChangeResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PriceChangeResponseDto>> ChangePrice(Guid id, [FromBody] decimal priceChange)
        {
            _logger.LogInformation(
                "Requête PATCH pour changer le prix du produit: {ProductId} à {NewPrice}",
                id,
                priceChange);

            try
            {
                var product = await _productService.ChangePriceAsync(id, priceChange);
                var response = new PriceChangeResponseDto
                {
                    ProductId = product.Id,
                    NewPrice = product.Price,
                    OldPrice = product.Price // à ajuster si l'ancien prix est conservé
                };
                return Ok(response);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du changement de prix pour le produit {ProductId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/discount")]
        [ProducesResponseType(typeof(DiscountResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DiscountResponseDto>> ApplyDiscount(Guid id, [FromBody] decimal discount)
        {
            _logger.LogInformation(
                "Requête PATCH pour appliquer une remise de {Discount}% sur le produit: {ProductId}",
                discount * 100,
                id);

            try
            {
                var product = await _productService.ApplyDiscountAsync(id, discount);
                var response = new DiscountResponseDto
                {
                    ProductId = product.Id,
                    DiscountPercentage = discount * 100,
                    NewPrice = product.Price,
                    OldPrice = product.Price
                };
                return Ok(response);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'application de la remise pour le produit {ProductId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductDto>> Activate(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour activer le produit: {ProductId}", id);

            try
            {
                var product = await _productService.ActivateAsync(id);
                return Ok(product);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'activation du produit {ProductId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductDto>> Deactivate(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour désactiver le produit: {ProductId}", id);

            try
            {
                var product = await _productService.DeactivateAsync(id);
                return Ok(product);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la désactivation du produit {ProductId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Requête DELETE pour le produit: {ProductId}", id);

            try
            {
                await _productService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du produit {ProductId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}