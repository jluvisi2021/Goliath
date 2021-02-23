using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    public class SignUpUserModel
    {
        [Required(ErrorMessage = "Please enter a username.")]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        [RegularExpression("^[a-zA-Z_]*$", ErrorMessage = "Username must be only letters and underscores.")]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 16 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        [Compare("ConfirmEmail", ErrorMessage = "Email does not match.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please confirm your email.")]
        [EmailAddress(ErrorMessage = "Please confirm your email.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm Email")]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        [DataType(DataType.Password)]
        [StringLength(2048, MinimumLength = 6, ErrorMessage = "Password length must be 6-2048 characters.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}