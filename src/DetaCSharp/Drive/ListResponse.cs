using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DetaCSharp.Drive
{
    public class ListResponse
    {
        public IEnumerable<string> Names { get; set; }
        public Paging Paging { get; set; }

    }

    public class Paging
    {
        public int? Size { get; set; }
        public string Last { get; set; }
    }
}
