using System.Net;

namespace Quidjibo.WebProxy.Models
{
    public class WebProxyResponse
    {
        public bool IsSuccessStatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }
    }

    public class WebProxyResponse<T> : WebProxyResponse
    {
        public T Data { get; set; }
    }
}