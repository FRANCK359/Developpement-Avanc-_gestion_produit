using Xunit;
using System;
using System.Threading.Tasks;
using AdvancedDevSample.Application.Services;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Domain.Entities;

namespace AdvancedDevSample.Tests.Application
{
    /// <summary>
    /// Tests unitaires pour le CustomerService
    /// </summary>
    public class CustomerServiceTests
    {
        private readonly Guid _customerId = Guid.NewGuid();

        [Fact]
        public async Task CreateCustomerDto_Initialization()
        {
            // Arrange
            var createDto = new CreateCustomerDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            // Act & Assert
            Assert.Equal("John", createDto.FirstName);
            Assert.Equal("Doe", createDto.LastName);
            Assert.Equal("john.doe@example.com", createDto.Email);
        }

        [Fact]
        public void Customer_Constructor_WithValidData_CreatesCustomer()
        {
            // Arrange & Act
            var customer = new Customer("John", "Doe", "john.doe@example.com");

            // Assert
            Assert.NotNull(customer);
            Assert.Equal("John", customer.FirstName);
            Assert.Equal("Doe", customer.LastName);
            Assert.Equal("john.doe@example.com".ToLower(), customer.Email);
            Assert.True(customer.IsActive);
        }

        [Fact]
        public void CustomerDto_Initialization()
        {
            // Arrange
            var customerDto = new CustomerDto
            {
                Id = _customerId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Assert
            Assert.Equal(_customerId, customerDto.Id);
            Assert.Equal("John", customerDto.FirstName);
            Assert.Equal("Doe", customerDto.LastName);
            Assert.Equal("john.doe@example.com", customerDto.Email);
            Assert.True(customerDto.IsActive);
        }

        [Fact]
        public void NotFoundException_Creation()
        {
            // Arrange
            var message = "Client non trouvé";

            // Act
            var exception = new NotFoundException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }
    }
}