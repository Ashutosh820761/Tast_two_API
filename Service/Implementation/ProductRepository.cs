using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Threading.Tasks;
using Tast_two_API.Data;
using Tast_two_API.Entity;
using Tast_two_API.Service.Interface;


namespace SHOPING.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly TestContext _context;
        private readonly Product product;

        public ProductRepository(TestContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.product.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.product.FindAsync(id);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            _context.product.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.product.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.product.FindAsync(id);
            if (product == null)
                return false;

            _context.product.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
