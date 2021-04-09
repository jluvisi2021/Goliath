using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    /// <summary>
    /// Manages the email verification page. <br /> Keeps track of if the user: <br />
    /// * Entered an Email <br />
    /// * Entered an Email that has already been confirmed. <br />
    /// * Has had an email sent to them.
    /// </summary>
    public class EmailConfirmModel
    {
        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        [StringLength(maximumLength: 40, MinimumLength = 3, ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsEmailSent { get; set; }
    }
}