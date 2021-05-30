using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetaCSharp.Base
{
    public class PutResponse
    {
        public ItemsResponse Processed { get; set; }
    }

    public class ItemsResponse
    {
        public IEnumerable<ItemKeyResponse> Items { get; set; }
    }
}
