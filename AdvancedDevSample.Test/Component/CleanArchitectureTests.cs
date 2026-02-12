using AdvancedDevSample.Api.Controllers;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Application.Services;
using AdvancedDevSample.Domain.Common;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Interfaces;
using AdvancedDevSample.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace AdvancedDevSample.Test.Component
{
    /// <summary>
    /// Tests de composant pour vérifier la Clean Architecture
    /// </summary>
    public class CleanArchitectureTests
    {
        private readonly Assembly _domainAssembly = typeof(BaseEntity).Assembly;
        private readonly Assembly _applicationAssembly = typeof(IProductService).Assembly;
        private readonly Assembly _infrastructureAssembly = typeof(EfProductRepository).Assembly;
        private readonly Assembly _apiAssembly = typeof(ProductsController).Assembly;

        [Fact]
        public void DomainLayer_ShouldNotReferenceOtherProjects()
        {
            // Arrange
            var referencedAssemblies = _domainAssembly.GetReferencedAssemblies()
                .Select(a => a.Name)
                .ToList();

            // Assert
            Assert.DoesNotContain("AdvancedDevSample.Application", referencedAssemblies);
            Assert.DoesNotContain("AdvancedDevSample.Infrastructure", referencedAssemblies);
            Assert.DoesNotContain("AdvancedDevSample.Api", referencedAssemblies);
        }

        [Fact]
        public void ApplicationLayer_ShouldReferenceDomainOnly()
        {
            // Arrange
            var referencedAssemblies = _applicationAssembly.GetReferencedAssemblies()
                .Select(a => a.Name)
                .ToList();

            // Assert
            Assert.Contains("AdvancedDevSample.Domain", referencedAssemblies);
            Assert.DoesNotContain("AdvancedDevSample.Infrastructure", referencedAssemblies);
            Assert.DoesNotContain("AdvancedDevSample.Api", referencedAssemblies);
        }

        [Fact]
        public void InfrastructureLayer_ShouldReferenceDomain()
        {
            // Arrange
            var referencedAssemblies = _infrastructureAssembly.GetReferencedAssemblies()
                .Select(a => a.Name)
                .ToList();

            // Assert
            Assert.Contains("AdvancedDevSample.Domain", referencedAssemblies);
        }

        [Fact]
        public void ApiLayer_ShouldReferenceAllLayers()
        {
            // Arrange
            var referencedAssemblies = _apiAssembly.GetReferencedAssemblies()
                .Select(a => a.Name)
                .ToList();

            // Assert
            Assert.Contains("AdvancedDevSample.Domain", referencedAssemblies);
            Assert.Contains("AdvancedDevSample.Application", referencedAssemblies);
            Assert.Contains("AdvancedDevSample.Infrastructure", referencedAssemblies);
        }

        [Fact]
        public void Entities_ShouldInheritFromBaseEntity()
        {
            // Arrange
            var entityTypes = _domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace?.Contains("Entities") == true)
                .Where(t => t.Name != "OrderItem" && t.Name != "ValueObject") // Exclure OrderItem et ValueObject
                .ToList();

            // Assert
            foreach (var entityType in entityTypes)
            {
                // Éviter les types de fermeture <>c
                if (entityType.Name.Contains("<"))
                    continue;

                Assert.True(typeof(BaseEntity).IsAssignableFrom(entityType),
                    $"{entityType.Name} should inherit from BaseEntity");
            }
        }

        [Fact]
        public void Repositories_ShouldImplementInterfaces()
        {
            // Arrange
            var repositoryTypes = _infrastructureAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"))
                .ToList();

            var interfaceTypes = _domainAssembly.GetTypes()
                .Where(t => t.IsInterface && t.Name.StartsWith("I") && t.Name.EndsWith("Repository"))
                .ToList();

            // Assert
            foreach (var repoType in repositoryTypes)
            {
                var matchingInterface = interfaceTypes
                    .FirstOrDefault(i => i.Name == $"I{repoType.Name}");

                if (matchingInterface != null)
                {
                    Assert.True(matchingInterface.IsAssignableFrom(repoType),
                        $"{repoType.Name} should implement {matchingInterface.Name}");
                }
            }
        }

        [Fact]
        public void Services_ShouldImplementInterfaces()
        {
            // Arrange
            var serviceTypes = _applicationAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
                .ToList();

            var interfaceTypes = _applicationAssembly.GetTypes()
                .Where(t => t.IsInterface && t.Name.StartsWith("I") && t.Name.EndsWith("Service"))
                .ToList();

            // Assert
            foreach (var serviceType in serviceTypes)
            {
                var matchingInterface = interfaceTypes
                    .FirstOrDefault(i => i.Name == $"I{serviceType.Name}");

                if (matchingInterface != null)
                {
                    Assert.True(matchingInterface.IsAssignableFrom(serviceType),
                        $"{serviceType.Name} should implement {matchingInterface.Name}");
                }
            }
        }

        [Fact]
        public void Controllers_ShouldInheritFromControllerBase()
        {
            // Arrange
            var controllerTypes = _apiAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller"))
                .ToList();

            // Assert
            foreach (var controllerType in controllerTypes)
            {
                Assert.True(typeof(ControllerBase).IsAssignableFrom(controllerType),
                    $"{controllerType.Name} should inherit from ControllerBase");
            }
        }

        [Fact]
        public void Controllers_ShouldHaveAuthorizeAttribute()
        {
            // Arrange
            var controllerTypes = _apiAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract &&
                       t.Name.EndsWith("Controller") &&
                       t.Name != "AuthController")
                .ToList();

            // Assert
            foreach (var controllerType in controllerTypes)
            {
                var hasAuthorize = controllerType.GetCustomAttributes(true)
                    .Any(a => a.GetType().Name == "AuthorizeAttribute");

                Assert.True(hasAuthorize,
                    $"{controllerType.Name} should have [Authorize] attribute");
            }
        }

        [Fact]
        public void DomainEvents_ShouldInheritFromDomainEvent()
        {
            // Arrange
            var eventTypes = _domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace?.Contains("Events") == true)
                .ToList();

            // Assert
            foreach (var eventType in eventTypes)
            {
                Assert.True(typeof(DomainEvent).IsAssignableFrom(eventType),
                    $"{eventType.Name} should inherit from DomainEvent");
            }
        }

        [Fact]
        public void Exceptions_ShouldInheritFromException()
        {
            // Arrange
            var exceptionTypes = _domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Exception"))
                .Union(_applicationAssembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Exception")))
                .ToList();

            // Assert
            foreach (var exceptionType in exceptionTypes)
            {
                Assert.True(typeof(Exception).IsAssignableFrom(exceptionType),
                    $"{exceptionType.Name} should inherit from Exception");
            }
        }

        [Fact]
        public void DTOs_ShouldBeSimpleClasses()
        {
            // Arrange
            var dtoTypes = _applicationAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace?.Contains("DTOs") == true)
                .ToList();

            // Assert
            foreach (var dtoType in dtoTypes)
            {
                Assert.False(typeof(BaseEntity).IsAssignableFrom(dtoType),
                    $"{dtoType.Name} should not inherit from BaseEntity");

                var properties = dtoType.GetProperties();
                Assert.NotEmpty(properties);
            }
        }
    }
}