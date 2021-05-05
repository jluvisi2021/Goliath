using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    /// <summary>
    /// Model for managing data for password resets.
    /// </summary>
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter a valid username.")]
        [DataType(DataType.Text)]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Please enter a valid username.")]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "Please enter a valid username.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        [StringLength(maximumLength: 40, MinimumLength = 3, ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }

        public bool IsEmailSent { get; set; }
    }
}