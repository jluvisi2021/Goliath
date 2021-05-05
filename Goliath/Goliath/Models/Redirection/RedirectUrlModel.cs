using System.ComponentModel.DataAnnotations;

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