using System.Net.Http;

namespace DetaCSharp.Utils
{
    public class RequestConfig
    {
        public string BaseUrl { get; set; }
        public object Body { get; set; }
        public HttpMethod Method { get; set; }
    }
}
