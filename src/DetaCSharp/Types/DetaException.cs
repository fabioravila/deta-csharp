using System;
using System.Net;

namespace DetaCSharp.Types
{
    public class DetaException : Exception
    {
        public DetaException(HttpStatusCode code, string message) : base(message)
        {
            Code = code;
        }

        public HttpStatusCode Code { get; }
    }
}
