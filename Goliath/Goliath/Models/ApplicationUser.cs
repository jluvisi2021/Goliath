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

        [Required]
        [MaxLength(22)] // "12/31/9999 12:59:59 PM"
        public string AccountCreationDate { get; set; }

        [MaxLength(22)] // "12/31/9999 12:59:59 PM"
        public string LastUserLogin { get; set; }

        /// <summary>
        /// The threshold of time until the user gets logged out.
        /// </summary>
        [Required]
        [MaxLength(2)]
        public int LogoutThreshold { get; set; }

        /// <summary>
        /// Represents a value when the user changes their email on their account but has not
        /// verified their new email yet.
        /// </summary>
        [MaxLength(40)]
        public string UnverifiedNewEmail { get; set; }

        [MaxLength(12)]
        public string UnverifiedNewPhone { get; set; }

        [Required]
        [MaxLength(22)] // "12/31/9999 12:59:59 PM"
        public string LastPasswordUpdate { get; set; }

        /// <summary>
        /// Stores a JSON that can be compiled into a list. <br /> The last 10 logins on the
        /// account, their IPs, and the time of occurrence.
        /// </summary>
        public string AccountLoginHistory { get; set; }

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