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
    /// Contrôleur pour la gestion des clients
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            ICustomerService customerService,
            ILogger<CustomersController> logger)
        {
            _customerService = customerService ??
                throw new ArgumentNullException(nameof(customerService));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> GetById(Guid id)
        {
            _logger.LogInformation("Requête GET pour le client avec l'ID: {CustomerId}", id);

            var customer = await _customerService.GetByIdAsync(id);

            return Ok(customer);
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> GetByEmail(string email)
        {
            _logger.LogInformation("Requête GET pour le client avec l'email: {Email}", email);

            var customer = await _customerService.GetByEmailAsync(email);

            return Ok(customer);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
        {
            _logger.LogInformation("Requête GET pour tous les clients");

            var customers = await _customerService.GetAllAsync();

            return Ok(customers);
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetActive()
        {
            _logger.LogInformation("Requête GET pour les clients actifs");

            var customers = await _customerService.GetActiveCustomersAsync();

            return Ok(customers);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] UpdateCustomerDto updateDto)
        {
            _logger.LogInformation("Requête PUT pour mettre à jour le client: {CustomerId}", id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerService.UpdateAsync(id, updateDto);

            return Ok(customer);
        }

        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerDto>> Activate(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour activer le client: {CustomerId}", id);

            var customer = await _customerService.ActivateAsync(id);

            return Ok(customer);
        }

        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerDto>> Deactivate(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour désactiver le client: {CustomerId}", id);

            var customer = await _customerService.DeactivateAsync(id);

            return Ok(customer);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Requête DELETE pour le client: {CustomerId}", id);

            await _customerService.DeleteAsync(id);

            return NoContent();
        }
    }
}