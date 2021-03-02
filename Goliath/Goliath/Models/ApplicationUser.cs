using Microsoft.AspNetCore.Identity;

namespace Goliath.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string BackgroundColor { get; set; }

        public string DarkTheme { get; set; }

        /// <summary>
        /// Manages all of the password data that the user has.
        /// JSON Format.
        /// </summary>
        public string UserData { get; set; }
    }
}