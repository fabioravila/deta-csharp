using DetaCSharp.Types;
using System.Net;

namespace DetaCSharp.Utils
{
    public class Response
    {
        public HttpStatusCode Status { get; set; }
        public object Body { get; set; }
        public DetaException Error { get; set; }

        public bool IsError => Error != null;
    }
}
