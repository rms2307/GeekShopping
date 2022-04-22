using GeekShopping.Web.Enum;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeekShopping.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _client;
        private readonly IRequestHelper _requestHelper;
        public const string BaseUri = "api/v1/product";

        public ProductService(HttpClient client, IRequestHelper requestHelper)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
        }

        public async Task<ProductViewModel> CreateProduct(ProductViewModel model, string token)
        {
            var response = await _requestHelper.ExecuteRequest(BaseUri, _client, HttpMethodEnum.Post, token, model);

            return JsonConvert.DeserializeObject<ProductViewModel>(response.Content);
        }

        public async Task<IEnumerable<ProductViewModel>> FindAllProducts(string token = "")
        {
            var response = await _requestHelper.ExecuteRequest(BaseUri, _client, HttpMethodEnum.Get, token, null);

            return JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(response.Content);
        }

        public async Task<ProductViewModel> FindProductById(long id, string token)
        {
            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/{id}", _client, HttpMethodEnum.Get, token, null);

            return JsonConvert.DeserializeObject<ProductViewModel>(response.Content);
        }

        public async Task<ProductViewModel> UpdateProduct(ProductViewModel model, string token)
        {
            var response = await _requestHelper.ExecuteRequest(BaseUri, _client, HttpMethodEnum.Put, token, model);

            return JsonConvert.DeserializeObject<ProductViewModel>(response.Content);
        }

        public async Task<bool> DeleteProductById(long id, string token)
        {
            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/{id}", _client, HttpMethodEnum.Delete, token, null);

            return JsonConvert.DeserializeObject<bool>(response.Content);
        }
    }
}
