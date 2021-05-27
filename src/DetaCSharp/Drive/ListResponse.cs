using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DetaCSharp.Drive
{
    public class ListResponse
    {
        [JsonPropertyName("names")]
        public IEnumerable<string> Names { get; set; }
        public Paging Paging { get; set; }

    }

    public class Paging
    {
        public int? Size { get; set; }
        public string Last { get; set; }
    }
}
