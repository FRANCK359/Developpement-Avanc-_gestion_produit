using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        public int TotalItems => Items.Sum(item => item.Quantity);
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
    }

    /// <summary>
    /// DTO pour la représentation d'un élément de commande
    /// </summary>
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalWithDiscount => SubTotal - Discount;
    }

    /// <summary>
    /// DTO pour la création d'une commande
    /// </summary>
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Le client est requis")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Au moins un produit est requis")]
        [MinLength(1, ErrorMessage = "La commande doit contenir au moins un produit")]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// DTO pour la création d'un élément de commande
    /// </summary>
    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "Le produit est requis")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "La quantité est requise")]
        [Range(1, 10000, ErrorMessage = "La quantité doit être entre 1 et 10000")]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO pour l'ajout d'un produit à une commande
    /// </summary>
    public class AddProductToOrderDto
    {
        [Required(ErrorMessage = "Le produit est requis")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "La quantité est requise")]
        [Range(1, 10000, ErrorMessage = "La quantité doit être entre 1 et 10000")]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO pour la mise à jour de la quantité d'un produit dans une commande
    /// </summary>
    public class UpdateOrderItemQuantityDto
    {
        [Required(ErrorMessage = "La quantité est requise")]
        [Range(1, 10000, ErrorMessage = "La quantité doit être entre 1 et 10000")]
        public int Quantity { get; set; }
    }
}