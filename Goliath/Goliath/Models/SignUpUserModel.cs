using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    /// <summary>
    /// Manages the registration page for clients making a new account.
    /// </summary>
    public class SignUpUserModel
    {
        /// <summary>
        /// Requirements: <br />
        /// - 6-18 Characters <br />
        /// - Matches a-Z, 0-9, _
        /// </summary>
        [Required(ErrorMessage = "Please enter a username.")]
        [DataType(DataType.Text)]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Username must be only letters, numbers, and underscores.")]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 16 characters.")]
        public string Username { get; set; }

        /// <summary>
        /// Email field. Uses default form email checking.
        /// </summary>
        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        [MaxLength(40, ErrorMessage = "Please enter a valid email.")]
        [Compare("ConfirmEmail", ErrorMessage = "Email does not match.")]
        public string Email { get; set; }

        /// <summary>
        /// Email field. Uses default form email checking.
        /// </summary>
        [Required(ErrorMessage = "Please confirm your email.")]
        [EmailAddress(ErrorMessage = "Please confirm your email.")]
        public string ConfirmEmail { get; set; }

        /// <summary>
        /// Requirements: <br />
        /// - 6-2048 characters <br />
        /// - Cannot contain spaces. <br />
        /// - One digit <br />
        /// - One UpperCase <br />
        /// - One LowerCase <br />
        /// - One Non-Alphanumeric Character <br />
        /// </summary>
        [Required(ErrorMessage = "Please enter a password.")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        [DataType(DataType.Password)]
        [StringLength(2048, MinimumLength = 6, ErrorMessage = "Password length must be 6-2048 characters.")]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "Password cannot contain spaces.")]
        public string Password { get; set; }

        /// <summary>
        /// Requirements: <br />
        /// - 6-2048 characters <br />
        /// - Cannot contain spaces. <br />
        /// - One digit <br />
        /// - One UpperCase <br />
        /// - One LowerCase <br />
        /// - One Non-Alphanumeric Character <br />
        /// </summary>
        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}