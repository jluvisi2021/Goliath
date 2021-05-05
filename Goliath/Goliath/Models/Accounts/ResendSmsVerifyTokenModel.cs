using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    public class ResendSmsVerifyTokenModel
    {
        [Required]
        public string Username { get; set; }

        public bool IsSuccess { get; set; }
    }
}