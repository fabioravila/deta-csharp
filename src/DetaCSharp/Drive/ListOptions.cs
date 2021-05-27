using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetaCSharp.Drive
{
    public class ListOptions
    {
        public string Prefix { get; set; }
        public int? Limit { get; set; }
        public string Last { get; set; }

        public static ListOptions Empty => new ListOptions();
    }
}
