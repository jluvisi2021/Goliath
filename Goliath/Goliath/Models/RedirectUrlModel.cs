using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Models
{
    public class RedirectUrlModel
    {
        [Required]
        [Url]
        public string Url { get; set; }

        [Required]
        [Url]
        public string ReturnUrl { get; set; }
    }
}
