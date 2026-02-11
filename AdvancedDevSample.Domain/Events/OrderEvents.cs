using System;
using AdvancedDevSample.Domain.Common;

namespace AdvancedDevSample.Domain.Events
{
    /// <summary>
    /// Événement déclenché lors de la création d'une commande
    /// </summary>
    public class OrderCreatedEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public Guid CustomerId { get; }
        public DateTime OrderDate { get; }

        public OrderCreatedEvent(AdvancedDevSample.Domain.Entities.Order order)
        {
            OrderId = order.Id;
            CustomerId = order.CustomerId;
            OrderDate = order.OrderDate;
        }
    }

    public class ProductAddedToOrderEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public ProductAddedToOrderEvent(Guid orderId, Guid productId, int quantity)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class ProductRemovedFromOrderEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public Guid ProductId { get; }

        public ProductRemovedFromOrderEvent(Guid orderId, Guid productId)
        {
            OrderId = orderId;
            ProductId = productId;
        }
    }

    public class OrderConfirmedEvent : DomainEvent
    {
        public Guid OrderId { get; }

        public OrderConfirmedEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }

    public class OrderCancelledEvent : DomainEvent
    {
        public Guid OrderId { get; }

        public OrderCancelledEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }

    public class OrderCompletedEvent : DomainEvent
    {
        public Guid OrderId { get; }

        public OrderCompletedEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }

    public class OrderTotalCalculatedEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public decimal TotalAmount { get; }

        public OrderTotalCalculatedEvent(Guid orderId, decimal totalAmount)
        {
            OrderId = orderId;
            TotalAmount = totalAmount;
        }
    }
}