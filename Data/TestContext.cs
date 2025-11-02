
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Options;
using Tast_two_API.Entity;

namespace Tast_two_API.Data
{
    public class TestContext : DbContext
    { 
        public TestContext(DbContextOptions<TestContext> options) : base(options) { }
        public DbSet<Product> product { get; set; }
    }
}
