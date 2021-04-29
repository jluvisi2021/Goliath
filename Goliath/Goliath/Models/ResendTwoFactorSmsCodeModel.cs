using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Models
{
    public class ResendTwoFactorSmsCodeModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }
    }
}
