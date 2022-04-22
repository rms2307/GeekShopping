using System.Net;

namespace GeekShopping.Web.Dto
{
    public class HttpResponseDto
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }
        public byte[] ContentBytes { get; set; }
    }
}
