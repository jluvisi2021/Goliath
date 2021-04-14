using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    /// <summary>
    /// Represents the custom user of a Goliath Service. <br /> All authentication is handled by
    /// .NET Core Identity.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(7)]
        public string BackgroundColor { get; set; }

        [Required]
        [MaxLength(5)]
        public string DarkTheme { get; set; }

        /// <summary>
        /// Stored in JSON. Notifications that get displayed to the user when they login.
        /// </summary>
        [Required]
        public string PendingNotifications { get; set; }

        /// <summary>
        /// Manages all of the password data that the user has. <br /><u> JSON Format </u>
        /// </summary>
        [Required]
        public string UserData { get; set; }
    }
}