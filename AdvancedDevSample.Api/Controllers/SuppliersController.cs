using AdvancedDevSample.Api.Filters;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedDevSample.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des fournisseurs
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController(
            ISupplierService supplierService,
            ILogger<SuppliersController> logger)
        {
            _supplierService = supplierService ??
                throw new ArgumentNullException(nameof(supplierService));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierDto>> GetById(Guid id)
        {
            _logger.LogInformation("Requête GET pour le fournisseur avec l'ID: {SupplierId}", id);

            var supplier = await _supplierService.GetByIdAsync(id);

            return Ok(supplier);
        }

        [HttpGet("name/{name}")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierDto>> GetByName(string name)
        {
            _logger.LogInformation("Requête GET pour le fournisseur avec le nom: {Name}", name);

            var supplier = await _supplierService.GetByNameAsync(name);

            return Ok(supplier);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SupplierDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
        {
            _logger.LogInformation("Requête GET pour tous les fournisseurs");

            var suppliers = await _supplierService.GetAllAsync();

            return Ok(suppliers);
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<SupplierDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetActive()
        {
            _logger.LogInformation("Requête GET pour les fournisseurs actifs");

            var suppliers = await _supplierService.GetActiveSuppliersAsync();

            return Ok(suppliers);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SupplierDto>> Create([FromBody] CreateSupplierDto createDto)
        {
            _logger.LogInformation("Requête POST pour créer un nouveau fournisseur: {Name}", createDto.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = await _supplierService.CreateAsync(createDto);

            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierDto>> Update(Guid id, [FromBody] UpdateSupplierDto updateDto)
        {
            _logger.LogInformation("Requête PUT pour mettre à jour le fournisseur: {SupplierId}", id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = await _supplierService.UpdateAsync(id, updateDto);

            return Ok(supplier);
        }

        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SupplierDto>> Activate(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour activer le fournisseur: {SupplierId}", id);

            var supplier = await _supplierService.ActivateAsync(id);

            return Ok(supplier);
        }

        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SupplierDto>> Deactivate(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour désactiver le fournisseur: {SupplierId}", id);

            var supplier = await _supplierService.DeactivateAsync(id);

            return Ok(supplier);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Requête DELETE pour le fournisseur: {SupplierId}", id);

            await _supplierService.DeleteAsync(id);

            return NoContent();
        }
    }
}