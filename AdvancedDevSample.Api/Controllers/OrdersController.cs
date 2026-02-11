
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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
            _orderService = orderService ??
                throw new ArgumentNullException(nameof(orderService));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetById(Guid id)
        {
            _logger.LogInformation("Requête GET pour la commande avec l'ID: {OrderId}", id);

            var order = await _orderService.GetByIdAsync(id);

            return Ok(order);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            _logger.LogInformation("Requête GET pour toutes les commandes");

            var orders = await _orderService.GetAllAsync();

            return Ok(orders);
        }

        [HttpGet("customer/{customerId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByCustomer(Guid customerId)
        {
            _logger.LogInformation("Requête GET pour les commandes du client: {CustomerId}", customerId);

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

            var order = await _orderService.CreateAsync(createDto);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

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

            var order = await _orderService.AddProductAsync(orderId, addProductDto);

            return Ok(order);
        }

        [HttpDelete("{orderId:guid}/products/{productId:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> RemoveProduct(Guid orderId, Guid productId)
        {
            _logger.LogInformation("Requête DELETE pour supprimer le produit {ProductId} de la commande {OrderId}", productId, orderId);

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
    }
}