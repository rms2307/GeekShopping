using GeekShopping.ProductApi.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekShooping.ProductApi.Model.Context
{
    public class ProductContext : DbContext
    {
        public ProductContext() { }

        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
