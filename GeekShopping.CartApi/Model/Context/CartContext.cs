using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartApi.Model.Context
{
    public class CartContext : DbContext
    {
        public CartContext(DbContextOptions<CartContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }		
        public DbSet<CartDetail> CartDetails { get; set; }		
        public DbSet<CartHeader> CartHeaders { get; set; }		
	}
}