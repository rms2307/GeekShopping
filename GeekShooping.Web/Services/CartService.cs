using GeekShopping.Web.Enum;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using GeekShopping.Web.Utils.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GeekShopping.Web.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _client;
        private readonly IRequestHelper _requestHelper;
        public const string BasePath = "api/v1/cart";

        public CartService(HttpClient client, IRequestHelper requestHelper)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
        }

        public async Task<CartViewModel> FindCartByUserId(string userId, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"{BasePath}/find-cart/{userId}");
            return await response.ReadContentAs<CartViewModel>();
        }

        public async Task<CartViewModel> AddItemToCart(CartViewModel model, string token)
        {
            return await _requestHelper.ExecuteRequest(_client, HttpMethodEnum.Post, model, $"{BasePath}/add-cart", token);
        }

        public async Task<CartViewModel> UpdateCart(CartViewModel model, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsJson($"{BasePath}/update-cart", model);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartViewModel>();
            else throw new Exception("Something went wrong when calling API"); ;
        }

        public async Task<bool> RemoveFromCart(long cartId, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.DeleteAsync($"{BasePath}/remove-cart/{cartId}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else throw new Exception("Something went wrong when calling API"); ;
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
