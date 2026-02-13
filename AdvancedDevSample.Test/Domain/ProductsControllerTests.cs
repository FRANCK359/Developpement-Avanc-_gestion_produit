using AdvancedDevSample.Api.Controllers;
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Application.Exceptions;
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
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<ILogger<ProductsController>> _loggerMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<ProductsController>>();

            _controller = new ProductsController(
                _productServiceMock.Object,
                _loggerMock.Object);
        }

        // ========================= GET BY ID =========================
        [Fact]
        public async Task GetById_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var product = new ProductDto { Id = id, Name = "Test Product" };

            _productServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(product);

            var result = await _controller.GetById(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();

            _productServiceMock
                .Setup(s => s.GetByIdAsync(id))
                .ThrowsAsync(new NotFoundException("Not found"));

            var result = await _controller.GetById(id);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ========================= GET ALL =========================
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var products = new List<ProductDto>
            {
                new ProductDto { Id = Guid.NewGuid(), Name = "A" },
                new ProductDto { Id = Guid.NewGuid(), Name = "B" }
            };

            _productServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(products, okResult.Value);
        }

        // ========================= CREATE =========================
        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Price = 100,
                SupplierId = Guid.NewGuid()
            };

            var productDto = new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "New Product",
                Price = 100,
                SupplierId = createDto.SupplierId
            };

            _productServiceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(productDto);

            var result = await _controller.Create(createDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(productDto, createdResult.Value);
        }

        // ========================= UPDATE =========================
        [Fact]
        public async Task Update_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var updateDto = new UpdateProductDto { Price = 120 };

            var productDto = new ProductDto { Id = id, Price = 120 };

            _productServiceMock.Setup(s => s.UpdateAsync(id, updateDto)).ReturnsAsync(productDto);

            var result = await _controller.Update(id, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(productDto, okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();
            var updateDto = new UpdateProductDto { Price = 120 };

            _productServiceMock
                .Setup(s => s.UpdateAsync(id, updateDto))
                .ThrowsAsync(new NotFoundException("Not found"));

            var result = await _controller.Update(id, updateDto);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ========================= DELETE =========================
        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var id = Guid.NewGuid();

            _productServiceMock.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();

            _productServiceMock
                .Setup(s => s.DeleteAsync(id))
                .ThrowsAsync(new NotFoundException("Not found"));

            var result = await _controller.Delete(id);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
