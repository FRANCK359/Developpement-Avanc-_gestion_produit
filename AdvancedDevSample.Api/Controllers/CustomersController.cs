// CustomersController.cs - VERSION REFACTORISÉE
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
    /// Contrôleur pour la gestion des clients
    /// </summary>
    public class CustomersController : BaseApiController<CustomersController>
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
            : base(logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
<<<<<<< Updated upstream
        public async Task<ActionResult<CustomerDto>> GetById(Guid id)
        {
            _logger.LogInformation("Requête GET pour le client avec l'ID: {CustomerId}", id);

            var customer = await _customerService.GetByIdAsync(id);

            return Ok(customer);
        }
=======
        public Task<ActionResult<CustomerDto>> GetById(Guid id) =>
            ExecuteAsync(() => _customerService.GetByIdAsync(id),
                "Requête GET pour le client avec l'ID: {CustomerId}", id);
>>>>>>> Stashed changes

        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<CustomerDto>> GetByEmail(string email) =>
            ExecuteAsync(() => _customerService.GetByEmailAsync(email),
                "Requête GET pour le client avec l'email: {Email}", email);

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<CustomerDto>>> GetAll() =>
            ExecuteAsync(() => _customerService.GetAllAsync(),
                "Requête GET pour tous les clients");

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<CustomerDto>>> GetActive() =>
            ExecuteAsync(() => _customerService.GetActiveCustomersAsync(),
                "Requête GET pour les clients actifs");

        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
<<<<<<< Updated upstream
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto createDto)
        {
            _logger.LogInformation("Requête POST pour créer un nouveau client: {Email}", createDto.Email);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerService.CreateAsync(createDto);

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }
=======
        public Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto createDto) =>
            ExecuteCreationAsync(
                () => _customerService.CreateAsync(createDto),
                customer => new { id = customer.Id },
                nameof(GetById),
                "Requête POST pour créer un nouveau client: {Email}", createDto.Email);
>>>>>>> Stashed changes

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] UpdateCustomerDto updateDto) =>
            ExecuteAsync(() => _customerService.UpdateAsync(id, updateDto),
                "Requête PUT pour mettre à jour le client: {CustomerId}", id);

        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        public Task<ActionResult<CustomerDto>> Activate(Guid id) =>
            ExecuteAsync(() => _customerService.ActivateAsync(id),
                "Requête PATCH pour activer le client: {CustomerId}", id);

        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        public Task<ActionResult<CustomerDto>> Deactivate(Guid id) =>
            ExecuteAsync(() => _customerService.DeactivateAsync(id),
                "Requête PATCH pour désactiver le client: {CustomerId}", id);

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
<<<<<<< Updated upstream
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Requête DELETE pour le client: {CustomerId}", id);

            await _customerService.DeleteAsync(id);

            return NoContent();
        }
=======
        public Task<IActionResult> Delete(Guid id) =>
            ExecuteDeleteAsync(() => _customerService.DeleteAsync(id),
                "Requête DELETE pour le client: {CustomerId}", id);
>>>>>>> Stashed changes
    }
}