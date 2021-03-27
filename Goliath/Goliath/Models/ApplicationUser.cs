using Microsoft.AspNetCore.Identity;

namespace Goliath.Models
{
    /// <summary>
    /// Represents the custom user of a Goliath Service. <br /> All authentication is handled by
    /// .NET Core Identity.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string BackgroundColor { get; set; }

        public string DarkTheme { get; set; }

        /// <summary>
        /// Manages all of the password data that the user has. <br /><u> JSON Format </u>
        /// </summary>
        public string UserData { get; set; }
    }
}