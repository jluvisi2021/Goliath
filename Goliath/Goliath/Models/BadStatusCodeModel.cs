using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Models
{
    public class BadStatusCodeModel
    {
        public string StatusCode { get; set; }
        public string LastPageVisited { get; set; }

        public string UserAgent { get; set; }
    }
}
