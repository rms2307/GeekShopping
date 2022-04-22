using GeekShopping.Web.Dto;
using GeekShopping.Web.Enum;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeekShopping.Web.Utils.Interfaces
{
    public interface IRequestHelper
    {
        Task<HttpResponseDto> ExecuteRequest(string url, HttpClient _client, HttpMethodEnum method, string jwtToken = null, object body = null);
    }
}
