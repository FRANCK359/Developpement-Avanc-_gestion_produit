using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Enums;
using AdvancedDevSample.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AdvancedDevSample.Application.Services
{
    /// <summary>
    /// Implémentation du service de gestion des commandes
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository ??
                throw new ArgumentNullException(nameof(orderRepository));
            _customerRepository = customerRepository ??
                throw new ArgumentNullException(nameof(customerRepository));
            _productRepository = productRepository ??
                throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderDto> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Récupération de la commande avec l'ID {OrderId}", id);

<<<<<<< Updated upstream
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {id} non trouvé");

=======
            var order = await GetOrderOrThrowAsync(id);
>>>>>>> Stashed changes
            return await MapToDtoAsync(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            _logger.LogInformation("Récupération de toutes les commandes");

            var orders = await _orderRepository.GetAllAsync();
            return await MapToDtoListAsync(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetByCustomerAsync(Guid customerId)
        {
            _logger.LogInformation("Récupération des commandes pour le client {CustomerId}", customerId);

            var orders = await _orderRepository.GetByCustomerAsync(customerId);
            return await MapToDtoListAsync(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetByStatusAsync(string status)
        {
            _logger.LogInformation("Récupération des commandes avec le statut {Status}", status);

            var orderStatus = ParseOrderStatus(status);
            var orders = await _orderRepository.GetByStatusAsync(orderStatus);

            return await MapToDtoListAsync(orders);
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto createDto)
        {
            _logger.LogInformation("Création d'une nouvelle commande pour le client {CustomerId}", createDto.CustomerId);

            await ValidateCustomerAsync(createDto.CustomerId);

            var order = new Order(createDto.CustomerId);
            await AddProductsToOrderAsync(order, createDto.Items);

<<<<<<< Updated upstream
            // Ajouter les produits si spécifiés
            if (createDto.Items != null && createDto.Items.Any())
            {
                foreach (var item in createDto.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                        throw new ValidationException($"Produit avec l'ID {item.ProductId} non trouvé");

                    order.AddProduct(product, item.Quantity);
                }
            }

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
=======
            await SaveOrderAsync(order);
>>>>>>> Stashed changes

            _logger.LogInformation("Commande créée avec succès: {OrderId}", order.Id);

            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> AddProductAsync(Guid orderId, AddProductToOrderDto addProductDto)
        {
            _logger.LogInformation("Ajout du produit {ProductId} à la commande {OrderId}",
                addProductDto.ProductId, orderId);

<<<<<<< Updated upstream
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {orderId} non trouvé");

            var product = await _productRepository.GetByIdAsync(addProductDto.ProductId);
            if (product == null)
                throw new ValidationException($"Produit avec l'ID {addProductDto.ProductId} non trouvé");
=======
            var order = await GetOrderOrThrowAsync(orderId);
            var product = await GetActiveProductAsync(addProductDto.ProductId);
>>>>>>> Stashed changes

            order.AddProduct(product, addProductDto.Quantity);
            await SaveOrderAsync(order);

            _logger.LogInformation("Produit ajouté avec succès à la commande {OrderId}", orderId);

            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> RemoveProductAsync(Guid orderId, Guid productId)
        {
<<<<<<< Updated upstream
            _logger.LogInformation("Suppression du produit {ProductId} de la commande {OrderId}", productId, orderId);

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {orderId} non trouvé");
=======
            _logger.LogInformation("Suppression du produit {ProductId} de la commande {OrderId}",
                productId, orderId);
>>>>>>> Stashed changes

            var order = await GetOrderOrThrowAsync(orderId);
            order.RemoveProduct(productId);
            await SaveOrderAsync(order);

            _logger.LogInformation("Produit supprimé avec succès de la commande {OrderId}", orderId);

            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> ConfirmAsync(Guid id)
        {
            _logger.LogInformation("Confirmation de la commande {OrderId}", id);

<<<<<<< Updated upstream
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {id} non trouvé");

=======
            var order = await GetOrderOrThrowAsync(id);
>>>>>>> Stashed changes
            order.Confirm();
            await SaveOrderAsync(order);

            _logger.LogInformation("Commande confirmée avec succès: {OrderId}", id);

            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> CancelAsync(Guid id)
        {
            _logger.LogInformation("Annulation de la commande {OrderId}", id);

<<<<<<< Updated upstream
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {id} non trouvé");

=======
            var order = await GetOrderOrThrowAsync(id);
>>>>>>> Stashed changes
            order.Cancel();
            await SaveOrderAsync(order);

            _logger.LogInformation("Commande annulée avec succès: {OrderId}", id);

            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> CompleteAsync(Guid id)
        {
            _logger.LogInformation("Complétion de la commande {OrderId}", id);

<<<<<<< Updated upstream
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {id} non trouvé");

=======
            var order = await GetOrderOrThrowAsync(id);
>>>>>>> Stashed changes
            order.Complete();
            await SaveOrderAsync(order);

            _logger.LogInformation("Commande complétée avec succès: {OrderId}", id);

            return await MapToDtoAsync(order);
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Suppression de la commande {OrderId}", id);

<<<<<<< Updated upstream
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {id} non trouvé");

=======
            await GetOrderOrThrowAsync(id);
>>>>>>> Stashed changes
            await _orderRepository.DeleteAsync(id);
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Commande supprimée avec succès: {OrderId}", id);
        }

        private async Task<Order> GetOrderOrThrowAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new NotFoundException("Order", id);
            }
            return order;
        }

        private static OrderStatus ParseOrderStatus(string status)
        {
            if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                throw new ValidationException("Status", $"Le statut '{status}' est invalide");
            }
            return orderStatus;
        }

        private async Task ValidateCustomerAsync(Guid customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                throw new NotFoundException("Customer", customerId);
            }

            if (!customer.IsActive)
            {
                throw new ValidationException("CustomerId", "Le client n'est pas actif");
            }
        }

        private async Task<Product> GetActiveProductAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                throw new NotFoundException("Product", productId);
            }

            if (!product.IsActive)
            {
                throw new ValidationException("ProductId", $"Le produit '{product.Name}' n'est pas actif");
            }

            return product;
        }

        private async Task AddProductsToOrderAsync(Order order, IEnumerable<CreateOrderItemDto> items)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            foreach (var item in items)
            {
                var product = await GetActiveProductAsync(item.ProductId);
                order.AddProduct(product, item.Quantity);
            }
        }

        private async Task SaveOrderAsync(Order order)
        {
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
        }

        private async Task<IEnumerable<OrderDto>> MapToDtoListAsync(IEnumerable<Order> orders)
        {
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                orderDtos.Add(await MapToDtoAsync(order));
            }

            return orderDtos;
        }

        private async Task<OrderDto> MapToDtoAsync(Order order)
        {
            var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
            var customerName = customer != null
                ? $"{customer.FirstName} {customer.LastName}"
                : "Client inconnu";

            var items = await MapOrderItemsAsync(order.OrderItems);

            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                CustomerId = order.CustomerId,
<<<<<<< Updated upstream
                CustomerName = string.Empty
=======
                CustomerName = customerName,
                Items = items.ToList()
>>>>>>> Stashed changes
            };
        }

        private async Task<IEnumerable<OrderItemDto>> MapOrderItemsAsync(IEnumerable<OrderItem> orderItems)
        {
            if (orderItems == null || !orderItems.Any())
            {
                return Enumerable.Empty<OrderItemDto>();
            }

<<<<<<< Updated upstream
            // Récupérer les détails des produits
            if (order.OrderItems != null && order.OrderItems.Count > 0)
=======
            var itemDtos = new List<OrderItemDto>();

            foreach (var item in orderItems)
>>>>>>> Stashed changes
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                itemDtos.Add(new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? "Produit inconnu",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.GetSubTotal()
                });
            }

            return itemDtos;
        }
    }
}