using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Application.Exceptions;

namespace AdvancedDevSample.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des commandes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Récupère une commande par son ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetById(Guid id)
        {
            _logger.LogInformation("Requête GET pour la commande avec l'ID: {OrderId}", id);

            try
            {
                var order = await _orderService.GetByIdAsync(id);
                return Ok(order);
            }
            catch (NotFoundException)
            {
                return NotFound(new { message = $"Commande avec l'ID {id} non trouvée" });
            }
        }

        /// <summary>
        /// Récupère toutes les commandes
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            _logger.LogInformation("Requête GET pour toutes les commandes");

            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Récupère les commandes d'un client
        /// </summary>
        [HttpGet("customer/{customerId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByCustomer(Guid customerId)
        {
            _logger.LogInformation("Requête GET pour les commandes du client: {CustomerId}", customerId);

            var orders = await _orderService.GetByCustomerAsync(customerId);
            return Ok(orders);
        }

        /// <summary>
        /// Récupère les commandes par statut
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByStatus(string status)
        {
            _logger.LogInformation("Requête GET pour les commandes avec le statut: {Status}", status);

            try
            {
                var orders = await _orderService.GetByStatusAsync(status);
                return Ok(orders);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crée une nouvelle commande
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto createDto)
        {
            _logger.LogInformation("Requête POST pour créer une nouvelle commande");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var order = await _orderService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Échec de création de commande: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Ajoute un produit à une commande
        /// </summary>
        [HttpPost("{orderId:guid}/products")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> AddProduct(Guid orderId, [FromBody] AddProductToOrderDto addProductDto)
        {
            _logger.LogInformation("Requête POST pour ajouter un produit à la commande: {OrderId}", orderId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var order = await _orderService.AddProductAsync(orderId, addProductDto);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Supprime un produit d'une commande
        /// </summary>
        [HttpDelete("{orderId:guid}/products/{productId:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> RemoveProduct(Guid orderId, Guid productId)
        {
            _logger.LogInformation("Requête DELETE pour supprimer le produit {ProductId} de la commande {OrderId}", productId, orderId);

            try
            {
                var order = await _orderService.RemoveProductAsync(orderId, productId);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Confirme une commande
        /// </summary>
        [HttpPatch("{id:guid}/confirm")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> Confirm(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour confirmer la commande: {OrderId}", id);

            try
            {
                var order = await _orderService.ConfirmAsync(id);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Annule une commande
        /// </summary>
        [HttpPatch("{id:guid}/cancel")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> Cancel(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour annuler la commande: {OrderId}", id);

            try
            {
                var order = await _orderService.CancelAsync(id);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Complète une commande
        /// </summary>
        [HttpPatch("{id:guid}/complete")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> Complete(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour compléter la commande: {OrderId}", id);

            try
            {
                var order = await _orderService.CompleteAsync(id);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Supprime une commande
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Requête DELETE pour la commande: {OrderId}", id);

            try
            {
                await _orderService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}