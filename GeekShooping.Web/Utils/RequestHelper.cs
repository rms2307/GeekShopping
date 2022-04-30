using GeekShopping.Web.Dto;
using GeekShopping.Web.Enum;
using GeekShopping.Web.Utils.Interfaces;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GeekShopping.Web.Utils
{
    public class RequestHelper : IRequestHelper
    {
        public async Task<HttpResponseDto> ExecuteRequest(string url, HttpClient client, HttpMethodEnum method, string jwtToken = null, object body = null)
        {
            var json = JsonConvert.SerializeObject(body);
            var data = MountContent(json);

            return await ExecuteRequest(url, client, data, method, jwtToken);
        }

        private async Task<HttpResponseDto> ExecuteRequest(string url, HttpClient client, StringContent data, HttpMethodEnum method, string jwtToken)
        {
            switch (method)
            {
                case HttpMethodEnum.Get:
                    {
                        var response = await Get(client, url, jwtToken);
                        client.Dispose();
                        return response;
                    }

                case HttpMethodEnum.Post:
                    {
                        var response = await Add(client, data, url, jwtToken);
                        client.Dispose();
                        return response;
                    }

                case HttpMethodEnum.Put:
                    {
                        var response = await Update(client, data, url, jwtToken);
                        client.Dispose();
                        return response;
                    }

                case HttpMethodEnum.Patch:
                    {
                        throw new NotImplementedException();
                    }

                case HttpMethodEnum.Delete:
                    {
                        var response = await Delete(client, url, jwtToken);
                        client.Dispose();
                        return response;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private async Task<HttpResponseDto> Get(HttpClient client, string uri = null, string jwtToken = null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await client.GetAsync(uri);
            return CreateResult(response);
        }

        private async Task<HttpResponseDto> Add(HttpClient client, StringContent data, string uri = null, string jwtToken = null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await client.PostAsync(uri, data);
            return CreateResult(response);
        }

        private async Task<HttpResponseDto> Update(HttpClient client, StringContent data, string uri = null, string jwtToken = null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await client.PutAsync(uri, data);
            if (response.IsSuccessStatusCode)
                return CreateResult(response);
            else throw new Exception("Something went wrong when calling API");
        }

        private async Task<HttpResponseDto> Delete(HttpClient client, string uri = null, string jwtToken = null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await client.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
                return CreateResult(response);
            else throw new Exception("Something went wrong when calling API");
        }

        private StringContent MountContent(string json, string mediaType = "application/json")
        {
            return new StringContent(json, Encoding.UTF8, mediaType);
        }

        private HttpResponseDto CreateResult(HttpResponseMessage response)
        {
            return new HttpResponseDto
            {
                StatusCode = response.StatusCode,
                Content = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult(),
                ContentBytes = response.Content.ReadAsByteArrayAsync().ConfigureAwait(false).GetAwaiter().GetResult(),
            };
        }
    }
}
