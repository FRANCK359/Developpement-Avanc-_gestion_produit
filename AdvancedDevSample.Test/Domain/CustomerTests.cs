using System;
using System.Linq;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Events;
using AdvancedDevSample.Domain.Exceptions;
using Xunit;

namespace AdvancedDevSample.Test.Domain
{
    public class CustomerTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateCustomer()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var email = "john.doe@example.com";

            // Act
            var customer = new Customer(firstName, lastName, email);

            // Assert
            Assert.NotNull(customer);
            Assert.NotEqual(Guid.Empty, customer.Id);
            Assert.Equal(firstName, customer.FirstName);
            Assert.Equal(lastName, customer.LastName);
            Assert.Equal(email.ToLower(), customer.Email);
            Assert.True(customer.IsActive);
            Assert.True((DateTime.UtcNow - customer.CreatedAt).TotalSeconds < 5);
            Assert.NotEmpty(customer.DomainEvents);
            Assert.Contains(typeof(CustomerCreatedEvent), customer.DomainEvents.Select(e => e.GetType()));
        }

        [Theory]
        [InlineData("", "Doe", "john@test.com")]
        [InlineData("   ", "Doe", "john@test.com")]
        public void Constructor_WithInvalidFirstName_ShouldThrowDomainException(string firstName, string lastName, string email)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Customer(firstName, lastName, email));
        }

        [Fact]
        public void Constructor_WithNullFirstName_ShouldThrowDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Customer(null!, "Doe", "john@test.com"));
        }

        [Theory]
        [InlineData("John", "", "john@test.com")]
        [InlineData("John", "   ", "john@test.com")]
        public void Constructor_WithInvalidLastName_ShouldThrowDomainException(string firstName, string lastName, string email)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Customer(firstName, lastName, email));
        }

        [Fact]
        public void Constructor_WithNullLastName_ShouldThrowDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Customer("John", null!, "john@test.com"));
        }

        [Theory]
        [InlineData("John", "Doe", "")]
        [InlineData("John", "Doe", "   ")]
        public void Constructor_WithEmptyEmail_ShouldThrowDomainException(string firstName, string lastName, string email)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Customer(firstName, lastName, email));
        }

        [Fact]
        public void Constructor_WithNullEmail_ShouldThrowDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Customer("John", "Doe", null!));
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("john.doe")]
        [InlineData("@example.com")]
        [InlineData("john@")]
        [InlineData("john@.com")]
        [InlineData("john@domain.")]
        [InlineData("john@domain..com")]
        public void Constructor_WithInvalidEmailFormat_ShouldThrowDomainException(string invalidEmail)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Customer("John", "Doe", invalidEmail));
        }

        [Fact]
        public void Activate_WhenInactive_ShouldActivateAndAddEvent()
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john@test.com");
            customer.Desactivate();
            var initialEventsCount = customer.DomainEvents.Count;

            // Act
            customer.Activate();

            // Assert
            Assert.True(customer.IsActive);
            Assert.Equal(initialEventsCount + 1, customer.DomainEvents.Count);
            Assert.Contains(typeof(CustomerActivatedEvent), customer.DomainEvents.Select(e => e.GetType()));
        }

        [Fact]
        public void Activate_WhenAlreadyActive_ShouldNotAddEvent()
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john@test.com");
            var initialEventsCount = customer.DomainEvents.Count;

            // Act
            customer.Activate();

            // Assert
            Assert.True(customer.IsActive);
            Assert.Equal(initialEventsCount, customer.DomainEvents.Count);
        }

        [Fact]
        public void Desactivate_WhenActive_ShouldDeactivateAndAddEvent()
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john@test.com");
            var initialEventsCount = customer.DomainEvents.Count;

            // Act
            customer.Desactivate();

            // Assert
            Assert.False(customer.IsActive);
            Assert.Equal(initialEventsCount + 1, customer.DomainEvents.Count);
            Assert.Contains(typeof(CustomerDeactivatedEvent), customer.DomainEvents.Select(e => e.GetType()));
        }

        [Fact]
        public void Desactivate_WhenAlreadyInactive_ShouldNotAddEvent()
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john@test.com");
            customer.Desactivate();
            var initialEventsCount = customer.DomainEvents.Count;

            // Act
            customer.Desactivate();

            // Assert
            Assert.False(customer.IsActive);
            Assert.Equal(initialEventsCount, customer.DomainEvents.Count);
        }

        [Fact]
        public void Update_WithValidParameters_ShouldUpdateCustomerAndAddEvent()
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john@test.com");
            var newFirstName = "Jane";
            var newLastName = "Smith";
            var newEmail = "jane.smith@test.com";
            var initialEventsCount = customer.DomainEvents.Count;

            // Act
            customer.Update(newFirstName, newLastName, newEmail);

            // Assert
            Assert.Equal(newFirstName, customer.FirstName);
            Assert.Equal(newLastName, customer.LastName);
            Assert.Equal(newEmail.ToLower(), customer.Email);
            Assert.Equal(initialEventsCount + 1, customer.DomainEvents.Count);
            Assert.Contains(typeof(CustomerUpdatedEvent), customer.DomainEvents.Select(e => e.GetType()));
        }

        [Fact]
        public void Update_WithSameEmail_ShouldUpdateWithoutDuplicateError()
        {
            // Arrange
            var email = "john@test.com";
            var customer = new Customer("John", "Doe", email);
            var initialEventsCount = customer.DomainEvents.Count;

            // Act
            customer.Update("John", "Doe", email);

            // Assert
            Assert.Equal(email.ToLower(), customer.Email);
            Assert.Equal(initialEventsCount + 1, customer.DomainEvents.Count);
        }

        [Theory]
        [InlineData("", "Smith", "jane@test.com")]
        [InlineData("Jane", "", "jane@test.com")]
        [InlineData("Jane", "Smith", "")]
        [InlineData("Jane", "Smith", "   ")]
        public void Update_WithInvalidParameters_ShouldThrowDomainException(string firstName, string lastName, string email)
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john@test.com");

            // Act & Assert
            Assert.Throws<DomainException>(() => customer.Update(firstName, lastName, email));
        }

        [Fact]
        public void Update_WithNullEmail_ShouldThrowDomainException()
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john@test.com");

            // Act & Assert
            Assert.Throws<DomainException>(() => customer.Update("Jane", "Smith", null!));
        }

        [Fact]
        public void Update_WithInvalidEmailFormat_ShouldThrowDomainException()
        {
            // Arrange
            var customer = new Customer("John", "Doe", "john@test.com");

            // Act & Assert
            Assert.Throws<DomainException>(() => customer.Update("Jane", "Smith", "invalid-email"));
        }

        [Fact]
        public void DomainEvents_ShouldContainCorrectEventTypes()
        {
            // Arrange & Act
            var customer = new Customer("John", "Doe", "john@test.com");
            customer.Update("Jane", "Smith", "jane@test.com");
            customer.Desactivate();
            customer.Activate();

            // Assert
            var eventTypes = customer.DomainEvents.Select(e => e.GetType()).ToList();
            Assert.Contains(typeof(CustomerCreatedEvent), eventTypes);
            Assert.Contains(typeof(CustomerUpdatedEvent), eventTypes);
            Assert.Contains(typeof(CustomerDeactivatedEvent), eventTypes);
            Assert.Contains(typeof(CustomerActivatedEvent), eventTypes);
        }
    }
}