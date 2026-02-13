using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedDevSample.Api.Controllers;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Exceptions;
using AdvancedDevSample.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdvancedDevSample.Test.Components
{
    public class ProductsControllerComponentTests
    {
        private readonly Mock<IProductService> _service;
        private readonly ProductsController _controller;

        public ProductsControllerComponentTests()
        {
            _service = new Mock<IProductService>();
            var logger = new Mock<ILogger<ProductsController>>();

            _controller = new ProductsController(_service.Object, logger.Object);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenProductExists()
        {
            var id = Guid.NewGuid();
            var product = new ProductDto { Id = id, Name = "Test", Price = 10 };

            _service.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(product);

            var result = await _controller.GetById(id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<ProductDto>(ok.Value);

            Assert.Equal(id, value.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var id = Guid.NewGuid();

            _service.Setup(x => x.GetByIdAsync(id))
                    .ThrowsAsync(new NotFoundException("Not found"));

            var result = await _controller.GetById(id);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var products = new List<ProductDto>
            {
                new ProductDto { Id = Guid.NewGuid(), Name = "P1" }
            };

            _service.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<ProductDto>>(ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var createDto = new CreateProductDto { Name = "New", Price = 20 };

            var product = new ProductDto { Id = Guid.NewGuid(), Name = "New" };

            _service.Setup(x => x.CreateAsync(createDto)).ReturnsAsync(product);

            var result = await _controller.Create(createDto);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var id = Guid.NewGuid();

            _service.Setup(x => x.DeleteAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
