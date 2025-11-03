using Microsoft.AspNetCore.Mvc;
using SHOPING.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tast_two_API.Entity;
using Tast_two_API.Service.Interface;
using Xunit;

namespace Shopping.Tests
{
    // ✅ Fake Repository implementing IProductRepository
    public class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new List<Product>();

        public Task<Product> AddProductAsync(Product product)
        {
            product.ProductId = _products.Count + 1;
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
                return Task.FromResult(false);

            _products.Remove(product);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return Task.FromResult<IEnumerable<Product>>(_products);
        }

        public Task<Product> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            return Task.FromResult(product);
        }

        public Task<Product> UpdateProductAsync(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Price = product.Price;
            }
            return Task.FromResult(product);
        }
    }

    // ✅ Actual Test Class
    public class ProductsControllerTests
    {
        private readonly ProductsController _controller;
        private readonly FakeProductRepository _fakeRepo;

        public ProductsControllerTests()
        {
            _fakeRepo = new FakeProductRepository();
            _controller = new ProductsController(_fakeRepo);
        }

        [Fact]
        public async Task Create_AddsProduct_ReturnsCreatedResult()
        {
            var product = new Product { Name = "Keyboard", Price = 1500 };

            var result = await _controller.Create(product);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal("Keyboard", returnValue.Name);
            Assert.Equal(1, returnValue.ProductId);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithList()
        {
            await _fakeRepo.AddProductAsync(new Product { Name = "Laptop", Price = 50000 });
            await _fakeRepo.AddProductAsync(new Product { Name = "Mouse", Price = 800 });

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, products.Count());
        }

        [Fact]
        public async Task GetById_ExistingProduct_ReturnsOk()
        {
            var product = await _fakeRepo.AddProductAsync(new Product { Name = "Tablet", Price = 12000 });

            var result = await _controller.GetById(product.ProductId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Product>(okResult.Value);
            Assert.Equal("Tablet", returnValue.Name);
        }

        [Fact]
        public async Task GetById_NonExistingProduct_ReturnsNotFound()
        {
            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_ValidProduct_ReturnsOk()
        {
            var product = await _fakeRepo.AddProductAsync(new Product { Name = "Phone", Price = 15000 });

            product.Name = "Smartphone";
            var result = await _controller.Update(product.ProductId, product);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<Product>(okResult.Value);
            Assert.Equal("Smartphone", updated.Name);
        }

        [Fact]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            var product = new Product { ProductId = 2, Name = "Monitor" };

            var result = await _controller.Update(1, product);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingProduct_ReturnsNoContent()
        {
            var product = await _fakeRepo.AddProductAsync(new Product { Name = "Charger", Price = 700 });

            var result = await _controller.Delete(product.ProductId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingProduct_ReturnsNotFound()
        {
            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
