using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Application.DTOs;

namespace AdvancedDevSample.Application.Interfaces.Services
{
    /// <summary>
    /// Service d'application pour la gestion des commandes
    /// </summary>
    public interface IOrderService
    {
        Task<OrderDto> GetByIdAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<IEnumerable<OrderDto>> GetByCustomerAsync(Guid customerId);
        Task<IEnumerable<OrderDto>> GetByStatusAsync(string status);
        Task<OrderDto> CreateAsync(CreateOrderDto createDto);
        Task<OrderDto> AddProductAsync(Guid orderId, AddProductToOrderDto addProductDto);
        Task<OrderDto> RemoveProductAsync(Guid orderId, Guid productId);
        Task<OrderDto> ConfirmAsync(Guid id);
        Task<OrderDto> CancelAsync(Guid id);
        Task<OrderDto> CompleteAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}