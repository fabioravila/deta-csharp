using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetaCSharp.Base
{
    public class QueryResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public Paging Paging { get; set; }

    }


    public class Paging
    {
        public int? Size { get; set; }
        public string Last { get; set; }
    }
}
