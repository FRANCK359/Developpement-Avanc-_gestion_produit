using System;
using System.Collections.Generic;

namespace AdvancedDevSample.Application.DTOs
{
    /// <summary>
    /// DTO pour la représentation d'une commande
    /// </summary>
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// DTO pour la représentation d'un élément de commande
    /// </summary>
    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }

    /// <summary>
    /// DTO pour la création d'une commande
    /// </summary>
    public class CreateOrderDto
    {
        public Guid CustomerId { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// DTO pour la création d'un élément de commande
    /// </summary>
    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO pour l'ajout d'un produit à une commande
    /// </summary>
    public class AddProductToOrderDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}