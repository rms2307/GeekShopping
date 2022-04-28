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
    public class CouponService : ICouponService
    {
        private readonly HttpClient _client;
        private readonly IRequestHelper _requestHelper;
        public const string BaseUri = "api/v1/coupon";

        public CouponService(HttpClient client, IRequestHelper requestHelper)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
        }
        public async Task<CouponViewModel> GetCoupon(string code, string token)
        {
            var response = await _requestHelper.ExecuteRequest($"{BaseUri}/{code}", _client, HttpMethodEnum.Get, token, null);
            if(response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<CouponViewModel>(response.Content);
        }
    }
}
