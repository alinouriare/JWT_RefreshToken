using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProduct.Mdoels
{
    public class Result
    {

        public string Action { get; set; }

        public bool Success { get { return Exception == null || Exception.Count == 0; } }

        public List<string> Exception { get; set; } = new List<string>();
    }
}
