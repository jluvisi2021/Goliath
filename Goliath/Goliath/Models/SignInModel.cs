using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    /// <summary>
    /// Model for signing in using the main Login page.
    /// </summary>
    public class SignInModel
    {
        /// <summary>
        /// The Username entered.
        /// </summary>
        [Display(Name = "Username")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        /// <summary>
        /// The password entered.
        /// </summary>
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Boolean representing the value of the Remember Me checkbox.
        /// </summary>
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}