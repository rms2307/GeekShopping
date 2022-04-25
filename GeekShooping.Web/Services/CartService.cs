using GeekShopping.Web.Enum;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils.Interfaces;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeekShopping.Web.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _client;
        private readonly IRequestHelper _requestHelper;
        public const string BaseUri = "api/v1/cart";

        public CartService(HttpClient client, IRequestHelper requestHelper)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
        }

        public async Task<CartViewModel> FindCartByUserId(string userId, string token)
        {
            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/find-cart/{userId}", _client, HttpMethodEnum.Get, token, null);

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

        public Task<bool> ApplyCoupon(CartViewModel cart, string couponCode, string token)
        {
            throw new System.NotImplementedException();
        }

        public Task<CartViewModel> Checkout(CartHeaderViewModel cartHeader, string token)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ClearCart(string userId, string token)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RemoveCoupon(string userId, string token)
        {
            throw new System.NotImplementedException();
        }
    }
}
