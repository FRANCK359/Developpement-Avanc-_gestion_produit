using System;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Events; // AJOUTER CETTE LIGNE
using AdvancedDevSample.Domain.Exceptions;
using Xunit;

namespace AdvancedDevSample.Test.Domain
{
    public class ProductTests
    {
        private readonly Guid _supplierId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateProduct()
        {
            // Arrange
            var name = "Laptop Gaming";
            var description = "PC portable haute performance";
            var price = 1299.99m;

            // Act
            var product = new Product(name, description, price, _supplierId);

            // Assert
            Assert.NotNull(product);
            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.Equal(name, product.Name);
            Assert.Equal(description, product.Description);
            Assert.Equal(price, product.Price);
            Assert.Equal(_supplierId, product.SupplierId);
            Assert.True(product.IsActive);
            Assert.True((DateTime.UtcNow - product.CreatedAt).TotalSeconds < 5);
            Assert.NotEmpty(product.DomainEvents);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidName_ShouldThrowDomainException(string invalidName)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new Product(invalidName, "Description", 100m, _supplierId));
        }

        [Fact]
        public void Constructor_WithNullName_ShouldThrowDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new Product(null!, "Description", 100m, _supplierId));
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-1)]
        [InlineData(-1000)]
        public void Constructor_WithNegativePrice_ShouldThrowDomainException(decimal invalidPrice)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new Product("Produit", "Description", invalidPrice, _supplierId));
        }

        [Fact]
        public void Constructor_WithZeroPrice_ShouldCreateProduct()
        {
            // Act
            var product = new Product("Produit", "Description", 0m, _supplierId);

            // Assert
            Assert.Equal(0m, product.Price);
        }

        [Fact]
        public void Constructor_WithEmptySupplierId_ShouldThrowDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new Product("Produit", "Description", 100m, Guid.Empty));
        }

        [Fact]
        public void ChangePrice_WithValidPrice_ShouldUpdatePriceAndAddEvent()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);
            var newPrice = 150m;
            var initialEventsCount = product.DomainEvents.Count;

            // Act
            product.ChangePrice(newPrice);

            // Assert
            Assert.Equal(newPrice, product.Price);
            Assert.Equal(initialEventsCount + 1, product.DomainEvents.Count);
        }

        [Fact]
        public void ChangePrice_WithSamePrice_ShouldNotAddEvent()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);
            var initialEventsCount = product.DomainEvents.Count;

            // Act
            product.ChangePrice(100m);

            // Assert
            Assert.Equal(100m, product.Price);
            Assert.Equal(initialEventsCount, product.DomainEvents.Count);
        }

        [Fact]
        public void ChangePrice_WithNegativePrice_ShouldThrowDomainException()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);

            // Act & Assert
            Assert.Throws<DomainException>(() => product.ChangePrice(-50m));
        }

        [Theory]
        [InlineData(0.1, 90)]    // 10% de réduction
        [InlineData(0.25, 75)]   // 25% de réduction
        [InlineData(0.5, 50)]    // 50% de réduction
        [InlineData(0.75, 25)]   // 75% de réduction
        [InlineData(1, 0)]       // 100% de réduction
        public void ApplyDiscount_WithValidDiscount_ShouldUpdatePriceAndAddEvent(decimal discount, decimal expectedPrice)
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);
            var initialEventsCount = product.DomainEvents.Count;

            // Act
            product.ApplyDiscount(discount);

            // Assert
            Assert.Equal(expectedPrice, product.Price);
            Assert.Equal(initialEventsCount + 1, product.DomainEvents.Count);
        }

        [Fact]
        public void ApplyDiscount_WithZeroDiscount_ShouldNotChangePriceOrAddEvent()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);
            var initialEventsCount = product.DomainEvents.Count;

            // Act
            product.ApplyDiscount(0);

            // Assert
            Assert.Equal(100m, product.Price);
            Assert.Equal(initialEventsCount, product.DomainEvents.Count);
        }

        [Theory]
        [InlineData(-0.1)]
        [InlineData(1.1)]
        [InlineData(2)]
        public void ApplyDiscount_WithInvalidDiscount_ShouldThrowDomainException(decimal invalidDiscount)
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);

            // Act & Assert
            Assert.Throws<DomainException>(() => product.ApplyDiscount(invalidDiscount));
        }

        [Fact]
        public void Activate_WhenInactive_ShouldActivateAndAddEvent()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);
            product.Desactivate();
            var initialEventsCount = product.DomainEvents.Count;

            // Act
            product.Activate();

            // Assert
            Assert.True(product.IsActive);
            Assert.Equal(initialEventsCount + 1, product.DomainEvents.Count);
        }

        [Fact]
        public void Activate_WhenAlreadyActive_ShouldNotAddEvent()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);
            var initialEventsCount = product.DomainEvents.Count;

            // Act
            product.Activate();

            // Assert
            Assert.True(product.IsActive);
            Assert.Equal(initialEventsCount, product.DomainEvents.Count);
        }

        [Fact]
        public void Desactivate_WhenActive_ShouldDeactivateAndAddEvent()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);
            var initialEventsCount = product.DomainEvents.Count;

            // Act
            product.Desactivate();

            // Assert
            Assert.False(product.IsActive);
            Assert.Equal(initialEventsCount + 1, product.DomainEvents.Count);
        }

        [Fact]
        public void Desactivate_WhenAlreadyInactive_ShouldNotAddEvent()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);
            product.Desactivate();
            var initialEventsCount = product.DomainEvents.Count;

            // Act
            product.Desactivate();

            // Assert
            Assert.False(product.IsActive);
            Assert.Equal(initialEventsCount, product.DomainEvents.Count);
        }
        [Fact]
        public void MultipleOperations_ShouldMaintainState()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);

            // Act
            product.ChangePrice(150m);     // +1 event
            product.ApplyDiscount(0.2m);   // +1 event (Price change)
            product.Desactivate();         // +1 event
            product.Activate();           // +1 event
            product.ChangePrice(200m);    // +1 event

            // Assert
            Assert.Equal(200m, product.Price);
            Assert.True(product.IsActive);
            Assert.Equal(6, product.DomainEvents.Count); // Created(1) + ChangePrice(2) + ApplyDiscount(1) + Desactivate(1) + Activate(1) = 6
        }

        [Fact]
        public void DomainEvents_ShouldContainCorrectEventTypes()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100m, _supplierId);

            // Act
            product.ChangePrice(150m);
            product.ApplyDiscount(0.1m);
            product.Desactivate();
            product.Activate();

            // Assert
            var eventTypes = product.DomainEvents.Select(e => e.GetType()).ToList();
            Assert.Contains(typeof(ProductCreatedEvent), eventTypes);
            Assert.Contains(typeof(ProductPriceChangedEvent), eventTypes);
            Assert.Contains(typeof(ProductDiscountAppliedEvent), eventTypes);
            Assert.Contains(typeof(ProductDeactivatedEvent), eventTypes);
            Assert.Contains(typeof(ProductActivatedEvent), eventTypes);
        }
    }
}