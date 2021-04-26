using Goliath.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Models
{
    public class TwoFactorModifyModel
    { 
        public string TwoFactorCode { get; set; }
        public string Password { get; set; }
    }
}
