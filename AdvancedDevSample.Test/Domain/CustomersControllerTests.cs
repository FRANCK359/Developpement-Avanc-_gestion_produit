using AdvancedDevSample.Api.Controllers;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AdvancedDevSample.Test.Domain
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mock<ILogger<CustomersController>> _loggerMock;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _loggerMock = new Mock<ILogger<CustomersController>>();

            _controller = new CustomersController(
                _customerServiceMock.Object,
                _loggerMock.Object);
        }

        // ====================== GetById ======================
        [Fact]
        public async Task GetById_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var customer = new CustomerDto { Id = id, Email = "test@mail.com" };

            _customerServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(customer);

            var result = await _controller.GetById(id);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(customer, okResult.Value);
        }

        // ====================== GetByEmail ======================
        [Fact]
        public async Task GetByEmail_ReturnsOk()
        {
            var email = "test@mail.com";
            var customer = new CustomerDto { Id = Guid.NewGuid(), Email = email };

            _customerServiceMock.Setup(s => s.GetByEmailAsync(email)).ReturnsAsync(customer);

            var result = await _controller.GetByEmail(email);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(customer, okResult.Value);
        }

        // ====================== GetAll ======================
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var customers = new List<CustomerDto>
            {
                new CustomerDto { Id = Guid.NewGuid(), Email = "a@mail.com" },
                new CustomerDto { Id = Guid.NewGuid(), Email = "b@mail.com" }
            };

            _customerServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(customers);

            var result = await _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(customers, okResult.Value);
        }

        // ====================== GetActive ======================
        [Fact]
        public async Task GetActive_ReturnsOk()
        {
            var customers = new List<CustomerDto>
            {
                new CustomerDto { Id = Guid.NewGuid(), Email = "a@mail.com" }
            };

            _customerServiceMock.Setup(s => s.GetActiveCustomersAsync()).ReturnsAsync(customers);

            var result = await _controller.GetActive();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(customers, okResult.Value);
        }

        // ====================== Create ======================
        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var createDto = new CreateCustomerDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@mail.com"
            };

            var customerDto = new CustomerDto
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john@mail.com"
            };

            _customerServiceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(customerDto);

            var result = await _controller.Create(createDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(customerDto, createdResult.Value);
        }

        // ====================== Update ======================
        [Fact]
        public async Task Update_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var updateDto = new UpdateCustomerDto
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@mail.com"
            };

            var customerDto = new CustomerDto
            {
                Id = id,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@mail.com"
            };

            _customerServiceMock.Setup(s => s.UpdateAsync(id, updateDto)).ReturnsAsync(customerDto);

            var result = await _controller.Update(id, updateDto);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(customerDto, okResult.Value);
        }

        // ====================== Activate ======================
        [Fact]
        public async Task Activate_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var customerDto = new CustomerDto { Id = id };

            _customerServiceMock.Setup(s => s.ActivateAsync(id)).ReturnsAsync(customerDto);

            var result = await _controller.Activate(id);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(customerDto, okResult.Value);
        }

        // ====================== Deactivate ======================
        [Fact]
        public async Task Deactivate_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var customerDto = new CustomerDto { Id = id };

            _customerServiceMock.Setup(s => s.DeactivateAsync(id)).ReturnsAsync(customerDto);

            var result = await _controller.Deactivate(id);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(customerDto, okResult.Value);
        }

        // ====================== Delete ======================
        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            _customerServiceMock.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
