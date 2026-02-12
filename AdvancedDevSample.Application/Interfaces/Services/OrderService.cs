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

            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {id} non trouvé");

            return await MapToDtoAsync(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            _logger.LogInformation("Récupération de toutes les commandes");

            var orders = await _orderRepository.GetAllAsync();

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(await MapToDtoAsync(order));
            }

            return orderDtos;
        }

        public async Task<IEnumerable<OrderDto>> GetByCustomerAsync(Guid customerId)
        {
            _logger.LogInformation("Récupération des commandes pour le client {CustomerId}", customerId);

            var orders = await _orderRepository.GetByCustomerAsync(customerId);

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(await MapToDtoAsync(order));
            }

            return orderDtos;
        }

        public async Task<IEnumerable<OrderDto>> GetByStatusAsync(string status)
        {
            _logger.LogInformation("Récupération des commandes avec le statut {Status}", status);

            if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                throw new ValidationException($"Statut invalide: {status}");

            var orders = await _orderRepository.GetByStatusAsync(orderStatus);

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(await MapToDtoAsync(order));
            }

            return orderDtos;
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto createDto)
        {
            _logger.LogInformation("Création d'une nouvelle commande pour le client {CustomerId}", createDto.CustomerId);

            // Vérifier si le client existe
            var customer = await _customerRepository.GetByIdAsync(createDto.CustomerId);
            if (customer == null)
                throw new ValidationException($"Client avec l'ID {createDto.CustomerId} non trouvé");

            if (!customer.IsActive)
                throw new ValidationException("Le client n'est pas actif");

            var order = new Order(createDto.CustomerId);

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

            _logger.LogInformation("Commande créée avec succès: {OrderId}", order.Id);

            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> AddProductAsync(Guid orderId, AddProductToOrderDto addProductDto)
        {
            _logger.LogInformation("Ajout du produit {ProductId} à la commande {OrderId}", addProductDto.ProductId, orderId);

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {orderId} non trouvé");

            var product = await _productRepository.GetByIdAsync(addProductDto.ProductId);
            if (product == null)
                throw new ValidationException($"Produit avec l'ID {addProductDto.ProductId} non trouvé");

            order.AddProduct(product, addProductDto.Quantity);

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Produit ajouté avec succès à la commande {OrderId}", orderId);

            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> RemoveProductAsync(Guid orderId, Guid productId)
        {
            _logger.LogInformation("Suppression du produit {ProductId} de la commande {OrderId}", productId, orderId);

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {orderId} non trouvé");

            order.RemoveProduct(productId);

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Produit supprimé avec succès de la commande {OrderId}", orderId);

            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> ConfirmAsync(Guid id)
        {
            _logger.LogInformation("Confirmation de la commande {OrderId}", id);

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {id} non trouvé");

            order.Confirm();

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Commande confirmée avec succès: {OrderId}", id);

            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> CancelAsync(Guid id)
        {
            _logger.LogInformation("Annulation de la commande {OrderId}", id);

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {id} non trouvé");

            order.Cancel();

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Commande annulée avec succès: {OrderId}", id);

            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> CompleteAsync(Guid id)
        {
            _logger.LogInformation("Complétion de la commande {OrderId}", id);

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {id} non trouvé");

            order.Complete();

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Commande complétée avec succès: {OrderId}", id);

            return await MapToDtoAsync(order);
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Suppression de la commande {OrderId}", id);

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Commande avec l'ID {id} non trouvé");

            await _orderRepository.DeleteAsync(id);
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Commande supprimée avec succès: {OrderId}", id);
        }

        private async Task<OrderDto> MapToDtoAsync(Order order)
        {
            var orderDto = new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                CustomerId = order.CustomerId,
                CustomerName = string.Empty
            };

            // Récupérer les détails du client
            var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
            if (customer != null)
            {
                orderDto.CustomerName = $"{customer.FirstName} {customer.LastName}";
            }

            // Récupérer les détails des produits
            if (order.OrderItems != null && order.OrderItems.Count > 0)
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    var itemDto = new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = product?.Name ?? "Produit inconnu",
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        SubTotal = item.GetSubTotal()
                    };
                    orderDto.Items.Add(itemDto);
                }
            }

            return orderDto;
        }
    }
}