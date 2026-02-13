// CustomerService.cs - VERSION REFACTORISÉE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Domain.Interfaces;
using AdvancedDevSample.Domain.Entities;

namespace AdvancedDevSample.Application.Services
{
    /// <summary>
    /// Implémentation du service de gestion des clients
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CustomerDto> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Récupération du client avec l'ID {CustomerId}", id);

            var customer = await GetCustomerOrThrowAsync(id);
            return MapToDto(customer);
        }

        public async Task<CustomerDto> GetByEmailAsync(string email)
        {
            _logger.LogInformation("Récupération du client avec l'email {Email}", email);

            var customer = await _customerRepository.GetByEmailAsync(email);
            if (customer == null)
            {
                throw new NotFoundException("Customer", email);
            }

            return MapToDto(customer);
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            _logger.LogInformation("Récupération de tous les clients");

            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(MapToDto);
        }

        public async Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync()
        {
            _logger.LogInformation("Récupération des clients actifs");

            var customers = await _customerRepository.GetActiveCustomersAsync();
            return customers.Select(MapToDto);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto createDto)
        {
            _logger.LogInformation("Création d'un nouveau client: {Email}", createDto.Email);

            await ValidateEmailNotExistsAsync(createDto.Email);

            var customer = new Customer(
                createDto.FirstName,
                createDto.LastName,
                createDto.Email);

            await SaveCustomerAsync(customer);

            _logger.LogInformation("Client créé avec succès: {CustomerId}", customer.Id);

            return MapToDto(customer);
        }

        public async Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto updateDto)
        {
            _logger.LogInformation("Mise à jour du client {CustomerId}", id);

            var customer = await GetCustomerOrThrowAsync(id);
            await ValidateEmailForUpdateAsync(updateDto.Email, id);

            customer.Update(updateDto.FirstName, updateDto.LastName, updateDto.Email);
            await SaveCustomerAsync(customer);

            _logger.LogInformation("Client mis à jour avec succès: {CustomerId}", id);

            return MapToDto(customer);
        }

        public async Task<CustomerDto> ActivateAsync(Guid id)
        {
            _logger.LogInformation("Activation du client {CustomerId}", id);

            var customer = await GetCustomerOrThrowAsync(id);
            customer.Activate();
            await SaveCustomerAsync(customer);

            _logger.LogInformation("Client activé avec succès: {CustomerId}", id);

            return MapToDto(customer);
        }

        public async Task<CustomerDto> DeactivateAsync(Guid id)
        {
            _logger.LogInformation("Désactivation du client {CustomerId}", id);

            var customer = await GetCustomerOrThrowAsync(id);
            customer.Desactivate();
            await SaveCustomerAsync(customer);

            _logger.LogInformation("Client désactivé avec succès: {CustomerId}", id);

            return MapToDto(customer);
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Suppression du client {CustomerId}", id);

            await GetCustomerOrThrowAsync(id);
            await _customerRepository.DeleteAsync(id);
            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation("Client supprimé avec succès: {CustomerId}", id);
        }

        private async Task<Customer> GetCustomerOrThrowAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                throw new NotFoundException("Customer", id);
            }
            return customer;
        }

        private async Task ValidateEmailNotExistsAsync(string email)
        {
            var existingCustomer = await _customerRepository.GetByEmailAsync(email);
            if (existingCustomer != null)
            {
                throw new ConflictException("Email", $"L'email {email} est déjà utilisé");
            }
        }

        private async Task ValidateEmailForUpdateAsync(string email, Guid currentCustomerId)
        {
            var existingCustomer = await _customerRepository.GetByEmailAsync(email);
            if (existingCustomer != null && existingCustomer.Id != currentCustomerId)
            {
                throw new ConflictException("Email", $"L'email {email} est déjà utilisé");
            }
        }

        private async Task SaveCustomerAsync(Customer customer)
        {
            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();
        }

        private static CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt
            };
        }
    }
}