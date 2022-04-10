using GeekShopping.Web.Enum;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeekShopping.Web.Utils.Interfaces
{
    public interface IRequestHelper
    {
        Task<T> ExecuteRequest<T>(HttpClient httpClient, HttpMethodEnum method, T body, string basePath = null, string jwtToken = null);
    }
}
