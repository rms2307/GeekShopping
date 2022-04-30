using GeekShopping.Web.Enum;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils.Interfaces;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeekShopping.Web.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _client;
        private readonly ICouponService _couponService;
        private readonly IRequestHelper _requestHelper;
        public const string BaseUri = "api/v1/cart";

        public CartService(HttpClient client, IRequestHelper requestHelper, ICouponService couponService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
            _couponService = couponService ?? throw new ArgumentNullException(nameof(couponService));
        }

        public async Task<CartViewModel> FindCartByUserId(string userId, string token)
        {
            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/find-cart/{userId}", _client, HttpMethodEnum.Get, token, null);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<CartViewModel>(response.Content);
        }

        public async Task<CartViewModel> AddItemToCart(CartViewModel model, string token)
        {
            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/add-cart", _client, HttpMethodEnum.Post, token, model);

            return JsonConvert.DeserializeObject<CartViewModel>(response.Content);
        }

        public async Task<CartViewModel> UpdateCart(CartViewModel model, string token)
        {
            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/update-cart", _client, HttpMethodEnum.Put, token, model);

            return JsonConvert.DeserializeObject<CartViewModel>(response.Content);
        }

        public async Task<bool> RemoveFromCart(long cartId, string token)
        {
            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/remove-cart/{cartId}", _client, HttpMethodEnum.Delete, token, null);

            return JsonConvert.DeserializeObject<bool>(response.Content);
        }

        public async Task<bool> ApplyCoupon(CartHeaderViewModel model, string token)
        {
            var hasCouponWithThisCode = await _couponService.GetCoupon(model.CouponCode, token);
            if (hasCouponWithThisCode?.CouponCode == null)
                return false;

            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/apply-coupon", _client, HttpMethodEnum.Post, token, model);

            return JsonConvert.DeserializeObject<bool>(response.Content);
        }

        public async Task<bool> RemoveCoupon(string userId, string token)
        {
            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/remove-coupon/{userId}", _client, HttpMethodEnum.Delete, token, null);

            return JsonConvert.DeserializeObject<bool>(response.Content);
        }

        public async Task<object> Checkout(CartHeaderViewModel model, string token)
        {
            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/checkout", _client, HttpMethodEnum.Post, token, model);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<CartHeaderViewModel>(response.Content);
            }
            else if (response.StatusCode.ToString().Equals("PreconditionFailed"))
            {
                return "Coupon Price has changed, please confirm!";
            }
            else throw new Exception("Something went wrong when calling API");
        }

        public Task<bool> ClearCart(string userId, string token)
        {
            throw new System.NotImplementedException();
        }

        public Task<object> Checkout(object cartHeader, string token)
        {
            throw new NotImplementedException();
        }
    }
}
