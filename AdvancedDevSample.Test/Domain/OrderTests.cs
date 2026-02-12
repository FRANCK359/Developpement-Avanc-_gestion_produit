using System;
using System.Linq;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Enums;
using AdvancedDevSample.Domain.Events;
using AdvancedDevSample.Domain.Exceptions;
using Xunit;

namespace AdvancedDevSample.Test.Domain
{
    public class OrderTests
    {
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _supplierId = Guid.NewGuid();

        private Product CreateValidProduct(decimal price = 100m, bool isActive = true)
        {
            var product = new Product("Produit Test", "Description", price, _supplierId);
            if (!isActive)
                product.Desactivate();
            return product;
        }

        [Fact]
        public void Constructor_WithValidCustomerId_ShouldCreateOrder()
        {
            // Act
            var order = new Order(_customerId);

            // Assert
            Assert.NotNull(order);
            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Equal(_customerId, order.CustomerId);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(0, order.TotalAmount);
            Assert.Empty(order.OrderItems);
            Assert.True((DateTime.UtcNow - order.OrderDate).TotalSeconds < 5);
            Assert.NotEmpty(order.DomainEvents);
            Assert.Contains(typeof(OrderCreatedEvent), order.DomainEvents.Select(e => e.GetType()));
        }

        [Fact]
        public void RemoveProduct_WithExistingProduct_ShouldRemoveProductFromOrder()
        {
            // Arrange
            var order = new Order(_customerId);
            var product1 = CreateValidProduct(50m);
            var product2 = CreateValidProduct(30m);
            order.AddProduct(product1, 2); // Total = 100
            order.AddProduct(product2, 3); // Total = 90 (100 + 90 = 190)
            var initialEventsCount = order.DomainEvents.Count;

            // Act
            order.RemoveProduct(product1.Id); // Supprime product1, reste product2 avec quantité 3 = 90

            // Assert
            Assert.Single(order.OrderItems);
            Assert.Equal(product2.Id, order.OrderItems.First().ProductId);
            Assert.Equal(3, order.OrderItems.First().Quantity);
            Assert.Equal(90m, order.TotalAmount); // 30 * 3 = 90
            Assert.Equal(initialEventsCount + 2, order.DomainEvents.Count); // RemoveProduct + CalculateTotal
        }

        [Fact]
        public void RemoveProduct_WithNonExistingProduct_ShouldThrowDomainException()
        {
            // Arrange
            var order = new Order(_customerId);
            var product = CreateValidProduct();
            order.AddProduct(product, 1);

            // Act & Assert
            Assert.Throws<DomainException>(() => order.RemoveProduct(Guid.NewGuid()));
        }

        [Fact]
        public void RemoveProduct_WhenOrderNotPending_ShouldThrowDomainException()
        {
            // Arrange
            var order = new Order(_customerId);
            var product = CreateValidProduct();
            order.AddProduct(product, 1);
            order.Confirm();

            // Act & Assert
            Assert.Throws<DomainException>(() => order.RemoveProduct(product.Id));
        }

        [Fact]
        public void Confirm_WithValidOrder_ShouldConfirmOrder()
        {
            // Arrange
            var order = new Order(_customerId);
            var product = CreateValidProduct();
            order.AddProduct(product, 2);
            var initialEventsCount = order.DomainEvents.Count;

            // Act
            order.Confirm();

            // Assert
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            Assert.Equal(product.Price * 2, order.TotalAmount);
            Assert.Equal(initialEventsCount + 1, order.DomainEvents.Count);
        }

        [Fact]
        public void Confirm_WithEmptyOrder_ShouldThrowDomainException()
        {
            // Arrange
            var order = new Order(_customerId);

            // Act & Assert
            Assert.Throws<DomainException>(() => order.Confirm());
        }

        [Fact]
        public void Confirm_WhenAlreadyConfirmed_ShouldThrowDomainException()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(CreateValidProduct(), 1);
            order.Confirm();

            // Act & Assert
            Assert.Throws<DomainException>(() => order.Confirm());
        }

        [Fact]
        public void Cancel_WhenPending_ShouldCancelOrder()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(CreateValidProduct(), 1);
            var initialEventsCount = order.DomainEvents.Count;

            // Act
            order.Cancel();

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal(initialEventsCount + 1, order.DomainEvents.Count);
        }

        [Fact]
        public void Cancel_WhenConfirmed_ShouldCancelOrder()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(CreateValidProduct(), 1);
            order.Confirm();

            // Act
            order.Cancel();

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }

        [Fact]
        public void Cancel_WhenAlreadyCancelled_ShouldThrowDomainException()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(CreateValidProduct(), 1);
            order.Cancel();

            // Act & Assert
            Assert.Throws<DomainException>(() => order.Cancel());
        }

        [Fact]
        public void Cancel_WhenCompleted_ShouldThrowDomainException()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(CreateValidProduct(), 1);
            order.Confirm();
            order.Complete();

            // Act & Assert
            Assert.Throws<DomainException>(() => order.Cancel());
        }

        [Fact]
        public void Complete_WhenConfirmed_ShouldCompleteOrder()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(CreateValidProduct(), 1);
            order.Confirm();
            var initialEventsCount = order.DomainEvents.Count;

            // Act
            order.Complete();

            // Assert
            Assert.Equal(OrderStatus.Completed, order.Status);
            Assert.Equal(initialEventsCount + 1, order.DomainEvents.Count);
        }

        [Fact]
        public void Complete_WhenNotConfirmed_ShouldThrowDomainException()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(CreateValidProduct(), 1);

            // Act & Assert
            Assert.Throws<DomainException>(() => order.Complete());
        }

        [Fact]
        public void CalculateTotal_ShouldUpdateTotalAmount()
        {
            // Arrange
            var order = new Order(_customerId);
            var product1 = CreateValidProduct(50m);
            var product2 = CreateValidProduct(30m);
            order.AddProduct(product1, 2); // 100
            order.AddProduct(product2, 3); // 90

            // Assert
            Assert.Equal(190m, order.TotalAmount);
        }

        [Fact]
        public void MultipleOperations_ShouldMaintainConsistency()
        {
            // Arrange
            var order = new Order(_customerId);
            var product1 = CreateValidProduct(100m);
            var product2 = CreateValidProduct(50m);

            // Act
            order.AddProduct(product1, 1);  // 100
            order.AddProduct(product2, 2);  // +100 = 200
            order.RemoveProduct(product1.Id); // -100 = 100
            order.AddProduct(product2, 1);  // +50 = 150
            order.Confirm();

            // Assert
            Assert.Single(order.OrderItems);
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            Assert.Equal(150m, order.TotalAmount);
            Assert.Equal(3, order.OrderItems.First().Quantity);
        }

        [Fact]
        public void DomainEvents_ShouldContainAllEventTypes()
        {
            // Arrange
            var order = new Order(_customerId);
            var product = CreateValidProduct();

            // Act
            order.AddProduct(product, 1);
            order.Confirm();
            order.Cancel();

            // Assert
            var eventTypes = order.DomainEvents.Select(e => e.GetType()).ToList();
            Assert.Contains(typeof(OrderCreatedEvent), eventTypes);
            Assert.Contains(typeof(ProductAddedToOrderEvent), eventTypes);
            Assert.Contains(typeof(OrderTotalCalculatedEvent), eventTypes);
            Assert.Contains(typeof(OrderConfirmedEvent), eventTypes);
            Assert.Contains(typeof(OrderCancelledEvent), eventTypes);
        }
    }
}