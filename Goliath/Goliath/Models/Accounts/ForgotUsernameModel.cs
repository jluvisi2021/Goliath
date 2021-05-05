using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    public class ForgotUsernameModel
    {
        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        [StringLength(maximumLength: 40, MinimumLength = 3, ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }

        public bool IsEmailSent { get; set; }
    }
}