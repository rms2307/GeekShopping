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

        private string _accessToken;
        private string _userId;

        public CartController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            _accessToken = await HttpContext.GetTokenAsync("access_token");
            _userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

            return View(await FindUserCart());
        }

        public async Task<IActionResult> Remove(int id)
        {
            if (await _cartService.RemoveFromCart(id, _accessToken))
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        private async Task<CartViewModel> FindUserCart()
        {
            var response = await _cartService.FindCartByUserId(_userId, _accessToken);

            if (response?.CartHeader != null)
            {
                CalculatePurchase(response);
            }

            return response;
        }

        private void CalculatePurchase(CartViewModel response)
        {
            foreach (var detail in response.CartDetails)
            {
                response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);
            }
        }
    }
}
