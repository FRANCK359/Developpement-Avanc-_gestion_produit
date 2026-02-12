using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Exceptions;
using System;
using Xunit;

namespace AdvancedDevSample.Tests.Domain
{
    /// <summary>
    /// Tests unitaires pour l'entité Customer
    /// </summary>
    public class CustomerTests
    {
        [Fact]
        public void Constructor_WithValidParameters_CreatesCustomer()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var email = "john.doe@example.com";

            // Act
            var customer = new Customer(firstName, lastName, email);

            // Assert
            Assert.NotNull(customer);
            Assert.Equal(firstName, customer.FirstName);
            Assert.Equal(lastName, customer.LastName);
            Assert.Equal(email.ToLower(), customer.Email);
            Assert.True(customer.IsActive);
            Assert.True((DateTime.UtcNow - customer.CreatedAt).TotalSeconds < 1);
        }

        [Theory]
        [InlineData("john.doe")]
        [InlineData("john.doe@")]
        [InlineData("@example.com")]
        [InlineData("")]
        public void Constructor_WithInvalidEmail_ThrowsDomainException(string invalidEmail)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new Customer("John", "Doe", invalidEmail));
        }

        [Fact]
        public void Activate_WhenInactive_ActivatesCustomer()
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john.doe@example.com");
            customer.Desactivate();

            // Act
            customer.Activate();

            // Assert
            Assert.True(customer.IsActive);
        }

        [Fact]
        public void Desactivate_WhenActive_DeactivatesCustomer()
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john.doe@example.com");

            // Act
            customer.Desactivate();

            // Assert
            Assert.False(customer.IsActive);
        }

        [Fact]
        public void Update_WithValidData_UpdatesCustomer()
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john.doe@example.com");
            var newFirstName = "Jane";
            var newLastName = "Smith";
            var newEmail = "jane.smith@example.com";

            // Act
            customer.Update(newFirstName, newLastName, newEmail);

            // Assert
            Assert.Equal(newFirstName, customer.FirstName);
            Assert.Equal(newLastName, customer.LastName);
            Assert.Equal(newEmail.ToLower(), customer.Email);
        }
    }
}