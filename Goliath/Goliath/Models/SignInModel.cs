using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    /// <summary>
    /// Model for signing in using the main Login page.
    /// </summary>
    public class SignInModel
    {

        [Required(ErrorMessage = "Please enter a valid username.")]
        [DataType(DataType.Text)]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Please enter a valid username.")]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "Please enter a valid username.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [StringLength(2048, MinimumLength = 6, ErrorMessage = "Please enter a valid password.")]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "Please enter a valid password.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Error. Please try again.")]
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}