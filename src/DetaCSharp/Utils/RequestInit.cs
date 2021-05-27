using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetaCSharp.Utils
{
    public class RequestInit
    {
        public object Payload { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
    }
}
