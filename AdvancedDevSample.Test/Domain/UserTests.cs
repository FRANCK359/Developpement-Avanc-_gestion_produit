using System;
using System.Linq;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Events;
using AdvancedDevSample.Domain.Exceptions;
using Xunit;

namespace AdvancedDevSample.Test.Domain
{
    public class UserTests
    {
        private readonly string _email = "user@example.com";
        private readonly string _passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");
        private readonly string _firstName = "John";
        private readonly string _lastName = "Doe";

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateUser()
        {
            // Act
            var user = new User(_email, _passwordHash, _firstName, _lastName);

            // Assert
            Assert.NotNull(user);
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.Equal(_email.ToLower(), user.Email);
            Assert.Equal(_passwordHash, user.PasswordHash);
            Assert.Equal(_firstName, user.FirstName);
            Assert.Equal(_lastName, user.LastName);
            Assert.Equal("User", user.Role);
            Assert.True(user.IsActive);
            Assert.True((DateTime.UtcNow - user.CreatedAt).TotalSeconds < 5);
            Assert.Null(user.LastLoginAt);
            Assert.NotEmpty(user.DomainEvents);
            Assert.Contains(typeof(UserCreatedEvent), user.DomainEvents.Select(e => e.GetType()));
        }
        [Fact]
        public void Constructor_ShouldAddUserCreatedEvent()
        {
            // Act
            var user = new User(_email, _passwordHash, _firstName, _lastName);

            // Assert
            Assert.Single(user.DomainEvents);
            Assert.IsType<UserCreatedEvent>(user.DomainEvents.First());
        }

        [Fact]
        public void Constructor_WithAdminRole_ShouldSetCorrectRole()
        {
            // Act
            var user = new User(_email, _passwordHash, _firstName, _lastName, "Admin");

            // Assert
            Assert.Equal("Admin", user.Role);
            Assert.Contains(typeof(UserCreatedEvent), user.DomainEvents.Select(e => e.GetType()));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidEmail_ShouldThrowDomainException(string invalidEmail)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new User(invalidEmail, _passwordHash, _firstName, _lastName));
        }

        [Fact]
        public void Constructor_WithNullEmail_ShouldThrowDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new User(null!, _passwordHash, _firstName, _lastName));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidPasswordHash_ShouldThrowDomainException(string invalidPasswordHash)
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new User(_email, invalidPasswordHash, _firstName, _lastName));
        }

        [Fact]
        public void Constructor_WithNullPasswordHash_ShouldThrowDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() =>
                new User(_email, null!, _firstName, _lastName));
        }

        [Fact]
        public void UpdateLastLogin_ShouldSetLastLoginDateAndAddEvent()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);
            Assert.Null(user.LastLoginAt);
            var initialEventsCount = user.DomainEvents.Count;

            // Act
            user.UpdateLastLogin();

            // Assert
            Assert.NotNull(user.LastLoginAt);
            Assert.True((DateTime.UtcNow - user.LastLoginAt.Value).TotalSeconds < 5);
            Assert.Equal(initialEventsCount + 1, user.DomainEvents.Count);
            Assert.Contains(typeof(UserLoggedInEvent), user.DomainEvents.Select(e => e.GetType()));
        }

        [Fact]
        public void ChangePassword_WithValidHash_ShouldUpdatePasswordAndAddEvent()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword("NewPassword456!");
            var initialEventsCount = user.DomainEvents.Count;

            // Act
            user.ChangePassword(newPasswordHash);

            // Assert
            Assert.Equal(newPasswordHash, user.PasswordHash);
            Assert.Equal(initialEventsCount + 1, user.DomainEvents.Count);
            Assert.Contains(typeof(UserPasswordChangedEvent), user.DomainEvents.Select(e => e.GetType()));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangePassword_WithInvalidHash_ShouldThrowDomainException(string invalidHash)
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);

            // Act & Assert
            Assert.Throws<DomainException>(() => user.ChangePassword(invalidHash));
        }

        [Fact]
        public void ChangePassword_WithNullHash_ShouldThrowDomainException()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);

            // Act & Assert
            Assert.Throws<DomainException>(() => user.ChangePassword(null!));
        }

        [Fact]
        public void UpdateProfile_WithValidData_ShouldUpdateProfileAndAddEvent()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);
            var newFirstName = "Jane";
            var newLastName = "Smith";
            var initialEventsCount = user.DomainEvents.Count;

            // Act
            user.UpdateProfile(newFirstName, newLastName);

            // Assert
            Assert.Equal(newFirstName, user.FirstName);
            Assert.Equal(newLastName, user.LastName);
            Assert.Equal(initialEventsCount + 1, user.DomainEvents.Count);
            Assert.Contains(typeof(UserProfileUpdatedEvent), user.DomainEvents.Select(e => e.GetType()));
        }

        [Fact]
        public void Activate_WhenInactive_ShouldActivateAndAddEvent()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);
            user.Deactivate();
            var initialEventsCount = user.DomainEvents.Count;

            // Act
            user.Activate();

            // Assert
            Assert.True(user.IsActive);
            Assert.Equal(initialEventsCount + 1, user.DomainEvents.Count);
            Assert.Contains(typeof(UserActivatedEvent), user.DomainEvents.Select(e => e.GetType()));
        }

        [Fact]
        public void Activate_WhenAlreadyActive_ShouldNotAddEvent()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);
            var initialEventsCount = user.DomainEvents.Count;

            // Act
            user.Activate();

            // Assert
            Assert.True(user.IsActive);
            Assert.Equal(initialEventsCount, user.DomainEvents.Count);
        }

        [Fact]
        public void Deactivate_WhenActive_ShouldDeactivateAndAddEvent()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);
            var initialEventsCount = user.DomainEvents.Count;

            // Act
            user.Deactivate();

            // Assert
            Assert.False(user.IsActive);
            Assert.Equal(initialEventsCount + 1, user.DomainEvents.Count);
            Assert.Contains(typeof(UserDeactivatedEvent), user.DomainEvents.Select(e => e.GetType()));
        }

        [Fact]
        public void Deactivate_WhenAlreadyInactive_ShouldNotAddEvent()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);
            user.Deactivate();
            var initialEventsCount = user.DomainEvents.Count;

            // Act
            user.Deactivate();

            // Assert
            Assert.False(user.IsActive);
            Assert.Equal(initialEventsCount, user.DomainEvents.Count);
        }

        [Fact]
        public void ChangeRole_ShouldUpdateRoleAndAddEvent()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);
            Assert.Equal("User", user.Role);
            var initialEventsCount = user.DomainEvents.Count;

            // Act
            user.ChangeRole("Manager");

            // Assert
            Assert.Equal("Manager", user.Role);
            Assert.Equal(initialEventsCount + 1, user.DomainEvents.Count);
            Assert.Contains(typeof(UserRoleChangedEvent), user.DomainEvents.Select(e => e.GetType()));
        }

        [Fact]
        public void FullUserLifecycle_ShouldWorkCorrectly()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);
            var initialEventsCount = user.DomainEvents.Count;

            // Act
            user.UpdateLastLogin();
            user.UpdateProfile("Jane", "Smith");
            user.ChangePassword(BCrypt.Net.BCrypt.HashPassword("NewPass123!"));
            user.ChangeRole("Admin");
            user.Deactivate();
            user.Activate();

            // Assert
            Assert.Equal("Jane", user.FirstName);
            Assert.Equal("Smith", user.LastName);
            Assert.Equal("Admin", user.Role);
            Assert.True(user.IsActive);
            Assert.NotNull(user.LastLoginAt);
            Assert.NotEqual(_passwordHash, user.PasswordHash);
            Assert.Equal(initialEventsCount + 6, user.DomainEvents.Count); // Created + 5 events
        }

        [Fact]
        public void DomainEvents_ShouldContainAllEventTypes()
        {
            // Arrange
            var user = new User(_email, _passwordHash, _firstName, _lastName);

            // Act
            user.UpdateLastLogin();
            user.UpdateProfile("Jane", "Smith");
            user.ChangePassword(BCrypt.Net.BCrypt.HashPassword("NewPass123!"));
            user.ChangeRole("Admin");
            user.Deactivate();
            user.Activate();

            // Assert
            var eventTypes = user.DomainEvents.Select(e => e.GetType()).ToList();
            Assert.Contains(typeof(UserCreatedEvent), eventTypes);
            Assert.Contains(typeof(UserLoggedInEvent), eventTypes);
            Assert.Contains(typeof(UserProfileUpdatedEvent), eventTypes);
            Assert.Contains(typeof(UserPasswordChangedEvent), eventTypes);
            Assert.Contains(typeof(UserRoleChangedEvent), eventTypes);
            Assert.Contains(typeof(UserDeactivatedEvent), eventTypes);
            Assert.Contains(typeof(UserActivatedEvent), eventTypes);
        }
    }
}