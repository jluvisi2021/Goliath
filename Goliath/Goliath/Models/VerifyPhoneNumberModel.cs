using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    public class VerifyPhoneNumberModel
    {
        [Required(ErrorMessage = "Please enter a token.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        public string Password { get; set; }

        public bool IsCompleted { get; set; }
    }
}