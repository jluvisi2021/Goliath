using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    /// <summary>
    /// The model which handles the actual reset screen where a user enters
    /// a new password.
    /// </summary>
    public class ResetPasswordModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Token { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Password does not match.")]
        [DataType(DataType.Password)]
        [StringLength(2048, MinimumLength = 6, ErrorMessage = "Password length must be 6-2048 characters.")]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "Password cannot contain spaces.")]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmNewPassword { get; set; }

        public bool IsCompleted { get; set; }
    }
}