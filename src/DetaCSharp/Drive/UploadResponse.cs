using DetaCSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetaCSharp.Drive
{
    public class UploadResponse
    {
        public object Response { get; set; }
        public DetaException Error { get; set; }
    }
}
