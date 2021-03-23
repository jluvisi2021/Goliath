using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    public class ForgotUsernameModel
    {
        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }

        public bool IsEmailSent { get; set; }
    }
}