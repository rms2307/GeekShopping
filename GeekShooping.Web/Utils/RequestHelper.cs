using GeekShopping.Web.Enum;
using GeekShopping.Web.Utils.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GeekShopping.Web.Utils
{
    public class RequestHelper : IRequestHelper
    {
        public async Task<T> ExecuteRequest<T>(HttpClient httpClient, HttpMethodEnum method, T body, string basePath = null, string jwtToken = null)
        {
            switch (method)
            {
                case HttpMethodEnum.Get:
                    {
                        throw new NotImplementedException();
                    }

                case HttpMethodEnum.Post:
                    {
                        return await Add(httpClient, body, basePath, jwtToken);
                    }

                case HttpMethodEnum.Put:
                    {
                        return await Update(httpClient, body, basePath, jwtToken);
                    }

                case HttpMethodEnum.Patch:
                    {
                        throw new NotImplementedException();
                    }

                case HttpMethodEnum.Delete:
                    {
                        throw new NotImplementedException();
                    }

                default:
                    throw new NotImplementedException();
            }
        }
        private async Task<T> Add<T>(HttpClient httpClient, T body, string basePath = null, string jwtToken = null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await httpClient.PostAsJson(basePath, body);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<T>();
            else throw new Exception("Something went wrong when calling API");
        }

        private async Task<T> Update<T>(HttpClient httpClient, T body, string basePath = null, string jwtToken = null)
        {
           httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await httpClient.PutAsJson(basePath, body);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<T>();
            else throw new Exception("Something went wrong when calling API");
        }
    }
}
