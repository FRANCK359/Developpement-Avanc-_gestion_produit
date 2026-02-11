using AdvancedDevSample.Domain;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Exceptions;
using System;
using Xunit;

namespace AdvancedDevSample.Tests.Domain
{
    /// <summary>
    /// Tests unitaires pour l'entité Product
    /// </summary>
    public class ProductTests
    {
        private readonly Guid _supplierId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidParameters_CreatesProduct()
        {
            // Arrange
            var name = "Produit Test";
            var description = "Description test";
            var price = 100.50m;

            // Act
            var product = new Product(name, description, price, _supplierId);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(name, product.Name);
            Assert.Equal(description, product.Description);
            Assert.Equal(price, product.Price);
            Assert.Equal(_supplierId, product.SupplierId);
            Assert.True(product.IsActive);
            Assert.True((DateTime.UtcNow - product.CreatedAt).TotalSeconds < 1);
        }

        [Theory]
        [InlineData("")]  // Chaîne vide
        [InlineData("   ")]  // Espaces
        public void Constructor_WithInvalidName_ThrowsDomainException(string invalidName)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new Product(invalidName, "Description", 100, _supplierId));
        }

        [Fact]
        public void Constructor_WithNullName_ThrowsDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new Product(null!, "Description", 100, _supplierId));
        }

        [Fact]
        public void ChangePrice_WithValidPrice_UpdatesPrice()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100, _supplierId);
            var newPrice = 150.75m;

            // Act
            product.ChangePrice(newPrice);

            // Assert
            Assert.Equal(newPrice, product.Price);
        }

        [Fact]
        public void ChangePrice_WithNegativePrice_ThrowsDomainException()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100, _supplierId);

            // Act & Assert
            Assert.Throws<DomainException>(() => product.ChangePrice(-50));
        }

        [Theory]
        [InlineData(0.1, 90)]    // 10% discount
        [InlineData(0.25, 75)]   // 25% discount
        [InlineData(0.5, 50)]    // 50% discount
        public void ApplyDiscount_WithValidDiscount_UpdatesPrice(decimal discount, decimal expectedPrice)
        {
            // Arrange
            var product = new Product("Produit", "Description", 100, _supplierId);

            // Act
            product.ApplyDiscount(discount);

            // Assert
            Assert.Equal(expectedPrice, product.Price);
        }

        [Theory]
        [InlineData(-0.1)]
        [InlineData(1.1)]
        public void ApplyDiscount_WithInvalidDiscount_ThrowsDomainException(decimal invalidDiscount)
        {
            // Arrange
            var product = new Product("Produit", "Description", 100, _supplierId);

            // Act & Assert
            Assert.Throws<DomainException>(() => product.ApplyDiscount(invalidDiscount));
        }

        [Fact]
        public void Activate_WhenInactive_ActivatesProduct()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100, _supplierId);
            product.Desactivate();

            // Act
            product.Activate();

            // Assert
            Assert.True(product.IsActive);
        }

        [Fact]
        public void Desactivate_WhenActive_DeactivatesProduct()
        {
            // Arrange
            var product = new Product("Produit", "Description", 100, _supplierId);

            // Act
            product.Desactivate();

            // Assert
            Assert.False(product.IsActive);
        }
    }
}