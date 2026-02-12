using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Enums;
using AdvancedDevSample.Domain.Exceptions;
using System;
using System.Linq;
using Xunit;

namespace AdvancedDevSample.Tests.Domain
{
    /// <summary>
    /// Tests unitaires pour l'entité Order
    /// </summary>
    public class OrderTests
    {
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Product _product;

        public OrderTests()
        {
            _product = new Product("Produit Test", "Description", 100, Guid.NewGuid());
        }

        [Fact]
        public void Constructor_WithValidCustomerId_CreatesOrder()
        {
            // Act
            var order = new Order(_customerId);

            // Assert
            Assert.NotNull(order);
            Assert.Equal(_customerId, order.CustomerId);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(0, order.TotalAmount);
            Assert.True((DateTime.UtcNow - order.OrderDate).TotalSeconds < 1);
            Assert.Empty(order.OrderItems);
        }

        [Fact]
        public void AddProduct_WithValidProduct_AddsProductToOrder()
        {
            // Arrange
            var order = new Order(_customerId);
            var quantity = 2;

            // Act
            order.AddProduct(_product, quantity);

            // Assert
            Assert.Single(order.OrderItems);
            var orderItem = order.OrderItems.First();
            Assert.Equal(_product.Id, orderItem.ProductId);
            Assert.Equal(quantity, orderItem.Quantity);
            Assert.Equal(_product.Price, orderItem.UnitPrice);
            Assert.Equal(_product.Price * quantity, order.TotalAmount);
        }

        [Fact]
        public void AddProduct_WithExistingProduct_UpdatesQuantity()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(_product, 2);

            // Act
            order.AddProduct(_product, 3);

            // Assert
            Assert.Single(order.OrderItems);
            var orderItem = order.OrderItems.First();
            Assert.Equal(5, orderItem.Quantity); // 2 + 3
            Assert.Equal(_product.Price * 5, order.TotalAmount);
        }

        [Fact]
        public void RemoveProduct_WithExistingProduct_RemovesProductFromOrder()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(_product, 2);

            // Act
            order.RemoveProduct(_product.Id);

            // Assert
            Assert.Empty(order.OrderItems);
            Assert.Equal(0, order.TotalAmount);
        }

        [Fact]
        public void Confirm_WhenPending_ConfirmsOrder()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(_product, 1);

            // Act
            order.Confirm();

            // Assert
            Assert.Equal(OrderStatus.Confirmed, order.Status);
        }

        [Fact]
        public void Confirm_WhenEmpty_ThrowsDomainException()
        {
            // Arrange
            var order = new Order(_customerId);

            // Act & Assert
            Assert.Throws<DomainException>(() => order.Confirm());
        }

        [Fact]
        public void Cancel_WhenNotCancelledOrCompleted_CancelsOrder()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(_product, 1);

            // Act
            order.Cancel();

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }

        [Fact]
        public void Complete_WhenConfirmed_CompletesOrder()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(_product, 1);
            order.Confirm();

            // Act
            order.Complete();

            // Assert
            Assert.Equal(OrderStatus.Completed, order.Status);
        }

        [Fact]
        public void AddProduct_WhenNotPending_ThrowsDomainException()
        {
            // Arrange
            var order = new Order(_customerId);
            order.AddProduct(_product, 1);
            order.Confirm();

            // Act & Assert
            Assert.Throws<DomainException>(() => order.AddProduct(_product, 1));
        }
    }
}