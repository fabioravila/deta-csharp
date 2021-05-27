using DetaCSharp.Types;
using System.IO;
using System.Net;

namespace DetaCSharp.Utils
{
    public class Response<T>
    {
        public HttpStatusCode Status { get; set; }
        public T Body { get; set; }
        public Stream BodyStream { get; set; }
        public DetaException Error { get; set; }

        public bool IsError => Error != null;

        public void EnsureSuccess()
        {
            if (Error != null)
            {
                throw Error;
            }
        }
    }

    public class Response : Response<Unit>
    {

    }

    public class Unit
    {

    }
}
