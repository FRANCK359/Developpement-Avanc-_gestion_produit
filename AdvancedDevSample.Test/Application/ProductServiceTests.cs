using Xunit;
using System;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Exceptions;

namespace AdvancedDevSample.Test.Application
{
    /// <summary>
    /// Tests unitaires pour le ProductService
    /// </summary>
    public class ProductServiceTests
    {
        [Fact]
        public void CreateProductDto_ShouldInitializeCorrectly()
        {
            // Arrange
            var supplierId = Guid.NewGuid();

            // Act
            var dto = new CreateProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                SupplierId = supplierId
            };

            // Assert
            Assert.Equal("Test Product", dto.Name);
            Assert.Equal("Test Description", dto.Description);
            Assert.Equal(99.99m, dto.Price);
            Assert.Equal(supplierId, dto.SupplierId);
        }

        [Fact]
        public void ProductDto_ShouldInitializeCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var supplierId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            // Act
            var dto = new ProductDto
            {
                Id = id,
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                IsActive = true,
                CreatedAt = now,
                SupplierId = supplierId
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal("Test Product", dto.Name);
            Assert.Equal("Test Description", dto.Description);
            Assert.Equal(99.99m, dto.Price);
            Assert.True(dto.IsActive);
            Assert.Equal(now, dto.CreatedAt);
            Assert.Equal(supplierId, dto.SupplierId);
        }

        [Fact]
        public void NotFoundException_ShouldStoreMessage()
        {
            // Arrange
            var message = "Product not found";

            // Act
            var exception = new NotFoundException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void ValidationException_ShouldStoreMessage()
        {
            // Arrange
            var message = "Validation failed";

            // Act
            var exception = new ValidationException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }
    }
}