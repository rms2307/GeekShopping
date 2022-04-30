using GeekShopping.CartApi.Data.ValueObjects;

namespace eekShopping.CartApi.Repository
{
    public interface ICouponRepository
    {
        Task<CouponVO> GetCouponByCouponCode(string couponCode, string token);
    }
}
