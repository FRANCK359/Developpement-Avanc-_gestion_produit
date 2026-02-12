using System;
using System.Linq;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Events;
using AdvancedDevSample.Domain.Exceptions;
using Xunit;

namespace AdvancedDevSample.Test.Domain
{
    public class SupplierTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateSupplier()
        {
            // Arrange
            var name = "Fournisseur Informatique";
            var contactEmail = "contact@fournisseur.com";

            // Act
            var supplier = new Supplier(name, contactEmail);

            // Assert
            Assert.NotNull(supplier);
            Assert.NotEqual(Guid.Empty, supplier.Id);
            Assert.Equal(name, supplier.Name);
            Assert.Equal(contactEmail.ToLower(), supplier.ContactEmail);
            Assert.True(supplier.IsActive);
            Assert.True((DateTime.UtcNow - supplier.CreatedAt).TotalSeconds < 5);
            Assert.NotEmpty(supplier.DomainEvents);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidName_ShouldThrowDomainException(string invalidName)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Supplier(invalidName, "contact@test.com"));
        }

        [Fact]
        public void Constructor_WithNullName_ShouldThrowDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Supplier(null!, "contact@test.com"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-email")]
        [InlineData("contact@")]
        [InlineData("@test.com")]
        [InlineData("contact@.com")]
        public void Constructor_WithInvalidEmail_ShouldThrowDomainException(string invalidEmail)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Supplier("Fournisseur", invalidEmail));
        }

        [Fact]
        public void Constructor_WithNullEmail_ShouldThrowDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Supplier("Fournisseur", null!));
        }

        // ... autres tests ...

        [Fact]
        public void Update_WithValidParameters_ShouldUpdateSupplierAndAddEvent()
        {
            // Arrange
            var supplier = new Supplier("Ancien Nom", "ancien@test.com");
            var newName = "Nouveau Fournisseur";
            var newEmail = "nouveau@test.com";
            var initialEventsCount = supplier.DomainEvents.Count;

            // Act
            supplier.Update(newName, newEmail);

            // Assert
            Assert.Equal(newName, supplier.Name);
            Assert.Equal(newEmail.ToLower(), supplier.ContactEmail);
            Assert.Equal(initialEventsCount + 1, supplier.DomainEvents.Count);
        }

        [Theory]
        [InlineData("", "email@test.com")]
        [InlineData("   ", "email@test.com")]
        public void Update_WithInvalidName_ShouldThrowDomainException(string name, string email)
        {
            // Arrange
            var supplier = new Supplier("Fournisseur", "contact@test.com");

            // Act & Assert
            Assert.Throws<DomainException>(() => supplier.Update(name, email));
        }

        [Fact]
        public void Update_WithNullName_ShouldThrowDomainException()
        {
            // Arrange
            var supplier = new Supplier("Fournisseur", "contact@test.com");

            // Act & Assert
            Assert.Throws<DomainException>(() => supplier.Update(null!, "email@test.com"));
        }

        [Theory]
        [InlineData("Fournisseur", "")]
        [InlineData("Fournisseur", "   ")]
        [InlineData("Fournisseur", "invalid-email")]
        public void Update_WithInvalidEmail_ShouldThrowDomainException(string name, string email)
        {
            // Arrange
            var supplier = new Supplier("Fournisseur", "contact@test.com");

            // Act & Assert
            Assert.Throws<DomainException>(() => supplier.Update(name, email));
        }

        [Fact]
        public void Update_WithNullEmail_ShouldThrowDomainException()
        {
            // Arrange
            var supplier = new Supplier("Fournisseur", "contact@test.com");

            // Act & Assert
            Assert.Throws<DomainException>(() => supplier.Update("Fournisseur", null!));
        }

        // ... reste du code ...
    }
}