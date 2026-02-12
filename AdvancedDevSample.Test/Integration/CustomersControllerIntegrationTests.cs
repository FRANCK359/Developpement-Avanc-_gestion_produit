using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdvancedDevSample.Api;
using AdvancedDevSample.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AdvancedDevSample.Test.Integration
{
    public class CustomersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _clientWithAuth;
        private readonly HttpClient _clientWithoutAuth;

        public CustomersControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            // CLIENT SANS AUTHENTIFICATION - Pour tester Unauthorized
            _clientWithoutAuth = factory.CreateClient();

            // CLIENT AVEC AUTHENTIFICATION - Pour tester OK
            _clientWithAuth = factory.CreateClient();
            _clientWithAuth.DefaultRequestHeaders.Add(
                "Authorization",
                $"{TestAuthHandler.AuthenticationScheme} test-token");
        }

        [Fact]
        public async Task GetAll_WithoutAuth_ShouldReturnUnauthorized()
        {
            // ARRANGE - Utiliser le client sans authentification
            var client = _clientWithoutAuth;

            // ACT
            var response = await client.GetAsync("/api/customers");

            // ASSERT
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_WithAuth_ShouldReturnOkAndCustomers()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            // ACT
            var response = await client.GetAsync("/api/customers");

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var customers = await response.Content.ReadFromJsonAsync<CustomerDto[]>();
            Assert.NotNull(customers);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnCustomer()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            // Créer d'abord un client
            var createDto = new CreateCustomerDto
            {
                FirstName = "Test",
                LastName = "Integration",
                Email = $"test.{Guid.NewGuid()}@example.com"
            };

            var createResponse = await client.PostAsJsonAsync("/api/customers", createDto);
            var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

            Assert.NotNull(createdCustomer);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            // ACT
            var response = await client.GetAsync($"/api/customers/{createdCustomer!.Id}");

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(customer);
            Assert.Equal(createdCustomer.Id, customer!.Id);
            Assert.Equal(createDto.Email.ToLower(), customer.Email);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            // ACT
            var response = await client.GetAsync($"/api/customers/{Guid.NewGuid()}");

            // ASSERT
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetByEmail_WithValidEmail_ShouldReturnCustomer()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            var email = $"test.{Guid.NewGuid()}@example.com";
            var createDto = new CreateCustomerDto
            {
                FirstName = "Test",
                LastName = "Email",
                Email = email
            };

            var createResponse = await client.PostAsJsonAsync("/api/customers", createDto);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            // ACT
            var response = await client.GetAsync($"/api/customers/email/{email}");

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(customer);
            Assert.Equal(email.ToLower(), customer!.Email);
        }

        [Fact]
        public async Task Create_WithValidData_ShouldReturnCreated()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            var createDto = new CreateCustomerDto
            {
                FirstName = "Nouveau",
                LastName = "Client",
                Email = $"nouveau.{Guid.NewGuid()}@example.com"
            };

            // ACT
            var response = await client.PostAsJsonAsync("/api/customers", createDto);

            // ASSERT
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(customer);
            Assert.Equal(createDto.FirstName, customer!.FirstName);
            Assert.Equal(createDto.LastName, customer.LastName);
            Assert.Equal(createDto.Email.ToLower(), customer.Email);
            Assert.True(customer.IsActive);
        }

        [Fact]
        public async Task Create_WithDuplicateEmail_ShouldReturnBadRequest()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            var email = $"duplicate.{Guid.NewGuid()}@example.com";
            var createDto1 = new CreateCustomerDto
            {
                FirstName = "Premier",
                LastName = "Client",
                Email = email
            };
            var response1 = await client.PostAsJsonAsync("/api/customers", createDto1);
            Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

            var createDto2 = new CreateCustomerDto
            {
                FirstName = "Second",
                LastName = "Client",
                Email = email
            };

            // ACT
            var response2 = await client.PostAsJsonAsync("/api/customers", createDto2);

            // ASSERT
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        }

        [Fact]
        public async Task Update_WithValidData_ShouldReturnOk()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            var createDto = new CreateCustomerDto
            {
                FirstName = "Ancien",
                LastName = "Nom",
                Email = $"update.{Guid.NewGuid()}@example.com"
            };
            var createResponse = await client.PostAsJsonAsync("/api/customers", createDto);
            var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(createdCustomer);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var updateDto = new UpdateCustomerDto
            {
                FirstName = "Nouveau",
                LastName = "Prenom",
                Email = $"nouveau.{Guid.NewGuid()}@example.com"
            };

            // ACT
            var response = await client.PutAsJsonAsync($"/api/customers/{createdCustomer!.Id}", updateDto);

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updatedCustomer = await response.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(updatedCustomer);
            Assert.Equal(updateDto.FirstName, updatedCustomer!.FirstName);
            Assert.Equal(updateDto.LastName, updatedCustomer.LastName);
            Assert.Equal(updateDto.Email.ToLower(), updatedCustomer.Email);
        }

        [Fact]
        public async Task Activate_ShouldActivateCustomer()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            var createDto = new CreateCustomerDto
            {
                FirstName = "Test",
                LastName = "Activation",
                Email = $"activate.{Guid.NewGuid()}@example.com"
            };
            var createResponse = await client.PostAsJsonAsync("/api/customers", createDto);
            var customer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(customer);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            // Désactiver d'abord
            var deactivateResponse = await client.PatchAsync($"/api/customers/{customer!.Id}/deactivate", null);
            Assert.Equal(HttpStatusCode.OK, deactivateResponse.StatusCode);

            // ACT
            var response = await client.PatchAsync($"/api/customers/{customer.Id}/activate", null);
            var activatedCustomer = await response.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(activatedCustomer);

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(activatedCustomer!.IsActive);
        }

        [Fact]
        public async Task Deactivate_ShouldDeactivateCustomer()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            var createDto = new CreateCustomerDto
            {
                FirstName = "Test",
                LastName = "Desactivation",
                Email = $"deactivate.{Guid.NewGuid()}@example.com"
            };
            var createResponse = await client.PostAsJsonAsync("/api/customers", createDto);
            var customer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(customer);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            // ACT
            var response = await client.PatchAsync($"/api/customers/{customer!.Id}/deactivate", null);
            var deactivatedCustomer = await response.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(deactivatedCustomer);

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(deactivatedCustomer!.IsActive);
        }

        [Fact]
        public async Task Delete_ShouldRemoveCustomer()
        {
            // ARRANGE - Utiliser le client avec authentification
            var client = _clientWithAuth;

            var createDto = new CreateCustomerDto
            {
                FirstName = "Test",
                LastName = "Suppression",
                Email = $"delete.{Guid.NewGuid()}@example.com"
            };
            var createResponse = await client.PostAsJsonAsync("/api/customers", createDto);
            var customer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(customer);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            // ACT
            var deleteResponse = await client.DeleteAsync($"/api/customers/{customer!.Id}");

            // ASSERT
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Vérifier que le client n'existe plus
            var getResponse = await client.GetAsync($"/api/customers/{customer.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}