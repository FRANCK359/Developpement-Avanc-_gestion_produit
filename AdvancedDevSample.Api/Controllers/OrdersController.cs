
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
<<<<<<< Updated upstream
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

=======
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
>>>>>>> Stashed changes

namespace AdvancedDevSample.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des commandes
    /// </summary>
    public class OrdersController : BaseApiController<OrdersController>
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
            : base(logger)
        {
<<<<<<< Updated upstream
            _orderService = orderService ??
                throw new ArgumentNullException(nameof(orderService));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
=======
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
>>>>>>> Stashed changes
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<OrderDto>> GetById(Guid id) =>
            ExecuteAsync(() => _orderService.GetByIdAsync(id),
                "Requête GET pour la commande avec l'ID: {OrderId}", id);

<<<<<<< Updated upstream
            var order = await _orderService.GetByIdAsync(id);

            return Ok(order);
        }

=======
>>>>>>> Stashed changes
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<OrderDto>>> GetAll() =>
            ExecuteAsync(() => _orderService.GetAllAsync(),
                "Requête GET pour toutes les commandes");

<<<<<<< Updated upstream
            var orders = await _orderService.GetAllAsync();

            return Ok(orders);
        }

=======
>>>>>>> Stashed changes
        [HttpGet("customer/{customerId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public Task<ActionResult<IEnumerable<OrderDto>>> GetByCustomer(Guid customerId) =>
            ExecuteAsync(() => _orderService.GetByCustomerAsync(customerId),
                "Requête GET pour les commandes du client: {CustomerId}", customerId);

<<<<<<< Updated upstream
            var orders = await _orderService.GetByCustomerAsync(customerId);

            return Ok(orders);
        }

        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByStatus(string status)
        {
            _logger.LogInformation("Requête GET pour les commandes avec le statut: {Status}", status);

            var orders = await _orderService.GetByStatusAsync(status);

            return Ok(orders);
        }

=======
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ActionResult<IEnumerable<OrderDto>>> GetByStatus(string status) =>
            ExecuteAsync(() => _orderService.GetByStatusAsync(status),
                "Requête GET pour les commandes avec le statut: {Status}", status);

>>>>>>> Stashed changes
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto createDto) =>
            ExecuteCreationAsync(
                () => _orderService.CreateAsync(createDto),
                order => new { id = order.Id },
                nameof(GetById),
                "Requête POST pour créer une nouvelle commande");

<<<<<<< Updated upstream
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _orderService.CreateAsync(createDto);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

=======
>>>>>>> Stashed changes
        [HttpPost("{orderId:guid}/products")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<OrderDto>> AddProduct(Guid orderId, [FromBody] AddProductToOrderDto addProductDto) =>
            ExecuteAsync(() => _orderService.AddProductAsync(orderId, addProductDto),
                "Requête POST pour ajouter un produit à la commande: {OrderId}", orderId);

<<<<<<< Updated upstream
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _orderService.AddProductAsync(orderId, addProductDto);

            return Ok(order);
        }

=======
>>>>>>> Stashed changes
        [HttpDelete("{orderId:guid}/products/{productId:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<OrderDto>> RemoveProduct(Guid orderId, Guid productId) =>
            ExecuteAsync(() => _orderService.RemoveProductAsync(orderId, productId),
                "Requête DELETE pour supprimer le produit {ProductId} de la commande {OrderId}", productId, orderId);

<<<<<<< Updated upstream
            var order = await _orderService.RemoveProductAsync(orderId, productId);

            return Ok(order);
        }

        [HttpPatch("{id:guid}/confirm")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<OrderDto>> Confirm(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour confirmer la commande: {OrderId}", id);

            var order = await _orderService.ConfirmAsync(id);

            return Ok(order);
        }

        [HttpPatch("{id:guid}/cancel")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<OrderDto>> Cancel(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour annuler la commande: {OrderId}", id);

            var order = await _orderService.CancelAsync(id);

            return Ok(order);
        }

        [HttpPatch("{id:guid}/complete")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<OrderDto>> Complete(Guid id)
        {
            _logger.LogInformation("Requête PATCH pour compléter la commande: {OrderId}", id);

            var order = await _orderService.CompleteAsync(id);

            return Ok(order);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Requête DELETE pour la commande: {OrderId}", id);

            await _orderService.DeleteAsync(id);

            return NoContent();
        }
=======
        [HttpPatch("{id:guid}/confirm")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<OrderDto>> Confirm(Guid id) =>
            ExecuteAsync(() => _orderService.ConfirmAsync(id),
                "Requête PATCH pour confirmer la commande: {OrderId}", id);

        [HttpPatch("{id:guid}/cancel")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<OrderDto>> Cancel(Guid id) =>
            ExecuteAsync(() => _orderService.CancelAsync(id),
                "Requête PATCH pour annuler la commande: {OrderId}", id);

        [HttpPatch("{id:guid}/complete")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<OrderDto>> Complete(Guid id) =>
            ExecuteAsync(() => _orderService.CompleteAsync(id),
                "Requête PATCH pour compléter la commande: {OrderId}", id);

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Delete(Guid id) =>
            ExecuteDeleteAsync(() => _orderService.DeleteAsync(id),
                "Requête DELETE pour la commande: {OrderId}", id);
>>>>>>> Stashed changes
    }
}