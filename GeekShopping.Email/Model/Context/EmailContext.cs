using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Model
{
    public class EmailContext : DbContext
    {
        public EmailContext(DbContextOptions<EmailContext> options) : base(options) { }
	
        public DbSet<EmailLog> Logs { get; set; }		
	}
}