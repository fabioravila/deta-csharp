using System.Collections.Generic;
using System.Linq;

namespace DetaCSharp.Utils
{
    public class RequestInit
    {
        public object Payload { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Headers => headersCollection.AsEnumerable();

        List<KeyValuePair<string, string>> headersCollection;
        public RequestInit()
        {
            headersCollection = new List<KeyValuePair<string, string>>();
        }


        public RequestInit AddHeader(string key, string value)
        {
            headersCollection.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }
    }
}
