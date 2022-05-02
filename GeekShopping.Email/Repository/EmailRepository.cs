using GeekShopping.Email.Messages;
using GeekShopping.Email.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<EmailContext> _context;

        public EmailRepository(DbContextOptions<EmailContext> context)
        {
            _context = context;
        }

        public async Task LogEmail(UpdatePaymentResultMessage message)
        {
            EmailLog log = new()
            {
                Email = message.Email,
                SentDate = DateTime.Now,
                Log = $"Order - {message.OrderId} has been created successfully!"
            };

            await using var _db = new EmailContext(_context);
            _db.Logs.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}
