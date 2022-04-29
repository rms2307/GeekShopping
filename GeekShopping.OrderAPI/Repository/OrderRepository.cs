using GeekShopping.OrderAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderApi.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<OrderContext> _context;

        public OrderRepository(DbContextOptions<OrderContext> context)
        {
            _context = context;
        }

        public async Task<bool> AddOrder(OrderHeader header)
        {
            if (header == null) return false;

            await using var _db = new OrderContext(_context);
            _db.Headers.Add(header);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateOrderPaymentStatus(long orderHeaderId, bool status)
        {
            await using var _db = new OrderContext(_context);
            var header = await _db.Headers
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderHeaderId);

            if (header != null)
            {
                header.PaymentStatus = status;
                await _db.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
