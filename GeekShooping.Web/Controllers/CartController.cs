using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace GeekShopping.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(
            IProductService productService, ICartService cartService, ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await FindUserCart());
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartViewModel model)
        {
            var credentials = await GetCredentials();

            bool response = await _cartService.ApplyCoupon(model.CartHeader, credentials.accessToken);
            if (response)
                return RedirectToAction(nameof(CartIndex));

            return View(nameof(CartIndex), await FindUserCart(false));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon()
        {
            var credentials = await GetCredentials();

            bool response = await _cartService.RemoveCoupon(credentials.userId, credentials.accessToken);
            if (response)
                return RedirectToAction(nameof(CartIndex));

            return View(nameof(CartIndex), await FindUserCart(false));
        }

        public async Task<IActionResult> Remove(int id)
        {
            var credentials = await GetCredentials();

            if (await _cartService.RemoveFromCart(id, credentials.accessToken))
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        public async Task<IActionResult> Checkout()
        {
            return View(await FindUserCart());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartViewModel model)
        {
            var credentials = await GetCredentials();

            var response = await _cartService.Checkout(model.CartHeader, credentials.accessToken);

            if (response != null && response.GetType() == typeof(string))
            {
                TempData["Error"] = response;
                return RedirectToAction(nameof(Checkout));
            }

            if (response != null)
                return RedirectToAction(nameof(Confirmation));

            return View(model);
        }

        public async Task<IActionResult> Confirmation()
        {
            return View();
        }

        private async Task<CartViewModel> FindUserCart(bool couponIsValid = true)
        {
            var credentials = await GetCredentials();

            CartViewModel response = await _cartService
                .FindCartByUserId(credentials.userId, credentials.accessToken);

            if (response?.CartHeader != null)
            {
                if (!string.IsNullOrEmpty(response.CartHeader.CouponCode))
                    await GetCouponAndSetDiscount(credentials, response);

                response.CouponIsValid = couponIsValid;

                CalculatePurchase(response);
            }

            return response;
        }

        private async Task GetCouponAndSetDiscount(dynamic credentials, CartViewModel response)
        {
            CouponViewModel coupon = await _couponService
                                    .GetCoupon(response.CartHeader.CouponCode, credentials.accessToken);
            if (coupon?.CouponCode != null)
                response.CartHeader.DiscountAmount = coupon.DiscountAmount;
        }

        private void CalculatePurchase(CartViewModel response)
        {
            foreach (var detail in response.CartDetails)
            {
                response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);
            }
            response.CartHeader.PurchaseAmount -= response.CartHeader.DiscountAmount;
        }

        private async Task<dynamic> GetCredentials()
        {
            return new
            {
                accessToken = await HttpContext.GetTokenAsync("access_token"),
                userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
            };
        }
    }
}
