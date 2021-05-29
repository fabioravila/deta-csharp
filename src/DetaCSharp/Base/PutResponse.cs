using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetaCSharp.Base
{
    public class PutResponse
    {
        public IEnumerable<ItemKeyResponse> Processed { get; set; }
    }
}
