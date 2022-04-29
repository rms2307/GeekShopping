using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Model
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }
	
        public DbSet<OrderDetail> Details { get; set; }		
        public DbSet<OrderHeader> Headers { get; set; }		
	}
}