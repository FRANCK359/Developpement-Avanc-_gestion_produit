// SuppliersController.cs - VERSION REFACTORISÉE
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
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
    public class SuppliersController : BaseApiController<SuppliersController>
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService, ILogger<SuppliersController> logger)
            : base(logger)
        {
            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<SupplierDto>> GetById(Guid id) =>
            ExecuteAsync(() => _supplierService.GetByIdAsync(id),
                "Requête GET pour le fournisseur avec l'ID: {SupplierId}", id);

        [HttpGet("name/{name}")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<SupplierDto>> GetByName(string name) =>
            ExecuteAsync(() => _supplierService.GetByNameAsync(name),
                "Requête GET pour le fournisseur avec le nom: {Name}", name);

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SupplierDto>), StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<SupplierDto>>> GetAll() =>
            ExecuteAsync(() => _supplierService.GetAllAsync(),
                "Requête GET pour tous les fournisseurs");

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<SupplierDto>), StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<SupplierDto>>> GetActive() =>
            ExecuteAsync(() => _supplierService.GetActiveSuppliersAsync(),
                "Requête GET pour les fournisseurs actifs");

        [HttpPost]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ActionResult<SupplierDto>> Create([FromBody] CreateSupplierDto createDto) =>
            ExecuteCreationAsync(
                () => _supplierService.CreateAsync(createDto),
                supplier => new { id = supplier.Id },
                nameof(GetById),
                "Requête POST pour créer un nouveau fournisseur: {Name}", createDto.Name);

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<SupplierDto>> Update(Guid id, [FromBody] UpdateSupplierDto updateDto) =>
            ExecuteAsync(() => _supplierService.UpdateAsync(id, updateDto),
                "Requête PUT pour mettre à jour le fournisseur: {SupplierId}", id);

        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        public Task<ActionResult<SupplierDto>> Activate(Guid id) =>
            ExecuteAsync(() => _supplierService.ActivateAsync(id),
                "Requête PATCH pour activer le fournisseur: {SupplierId}", id);

        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        public Task<ActionResult<SupplierDto>> Deactivate(Guid id) =>
            ExecuteAsync(() => _supplierService.DeactivateAsync(id),
                "Requête PATCH pour désactiver le fournisseur: {SupplierId}", id);

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Delete(Guid id) =>
            ExecuteDeleteAsync(() => _supplierService.DeleteAsync(id),
                "Requête DELETE pour le fournisseur: {SupplierId}", id);
    }
}