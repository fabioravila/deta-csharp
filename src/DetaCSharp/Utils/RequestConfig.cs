using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DetaCSharp.Utils
{
    public class RequestConfig
    {
        public string BaseUrl { get; set; }
        public object Body { get; set; }
        public HttpMethod Method { get; set; }
    }
}
