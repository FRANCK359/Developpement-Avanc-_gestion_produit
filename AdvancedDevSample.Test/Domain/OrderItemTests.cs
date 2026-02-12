using System;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Exceptions;
using Xunit;

namespace AdvancedDevSample.Test.Domain
{
    public class OrderItemTests
    {
        private readonly Guid _productId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateOrderItem()
        {
            // Arrange
            var quantity = 5;
            var unitPrice = 29.99m;

            // Act
            var orderItem = new OrderItem(_productId, quantity, unitPrice);

            // Assert
            Assert.NotNull(orderItem);
            Assert.NotEqual(Guid.Empty, orderItem.Id);
            Assert.Equal(_productId, orderItem.ProductId);
            Assert.Equal(quantity, orderItem.Quantity);
            Assert.Equal(unitPrice, orderItem.UnitPrice);
        }
        [Fact]
        public void Constructor_WithEmptyProductId_ShouldThrowDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new OrderItem(Guid.Empty, 1, 10m));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5)]
        public void Constructor_WithInvalidQuantity_ShouldThrowDomainException(int quantity)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new OrderItem(_productId, quantity, 10m));
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_WithNegativeUnitPrice_ShouldThrowDomainException(decimal unitPrice)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new OrderItem(_productId, 1, unitPrice));
        }

        [Fact]
        public void Constructor_WithZeroUnitPrice_ShouldCreateOrderItem()
        {
            // Act
            var orderItem = new OrderItem(_productId, 1, 0m);

            // Assert
            Assert.Equal(0m, orderItem.UnitPrice);
        }

        [Fact]
        public void GetSubTotal_ShouldCalculateCorrectly()
        {
            // Arrange
            var quantity = 3;
            var unitPrice = 15.50m;
            var orderItem = new OrderItem(_productId, quantity, unitPrice);

            // Act
            var subTotal = orderItem.GetSubTotal();

            // Assert
            Assert.Equal(quantity * unitPrice, subTotal);
        }

        [Fact]
        public void UpdateQuantity_WithValidQuantity_ShouldUpdate()
        {
            // Arrange
            var orderItem = new OrderItem(_productId, 2, 10m);
            var newQuantity = 5;

            // Act
            orderItem.UpdateQuantity(newQuantity);

            // Assert
            Assert.Equal(newQuantity, orderItem.Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void UpdateQuantity_WithInvalidQuantity_ShouldThrowDomainException(int newQuantity)
        {
            // Arrange
            var orderItem = new OrderItem(_productId, 2, 10m);

            // Act & Assert
            Assert.Throws<DomainException>(() => orderItem.UpdateQuantity(newQuantity));
        }

        [Fact]
        public void MultipleUpdates_ShouldWorkCorrectly()
        {
            // Arrange
            var orderItem = new OrderItem(_productId, 1, 10m);
            Assert.Equal(1, orderItem.Quantity);

            // Act - Mise à jour à 3
            orderItem.UpdateQuantity(3);
            Assert.Equal(3, orderItem.Quantity);
            var subTotal1 = orderItem.GetSubTotal(); // 30

            // Act - Mise à jour à 5
            orderItem.UpdateQuantity(5);
            Assert.Equal(5, orderItem.Quantity);
            var subTotal2 = orderItem.GetSubTotal(); // 50

            // Assert
            Assert.Equal(5, orderItem.Quantity);
            Assert.Equal(30m, subTotal1);
            Assert.Equal(50m, subTotal2);
        }
    }
}