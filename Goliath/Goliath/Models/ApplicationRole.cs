using Microsoft.AspNetCore.Identity;

namespace Goliath.Models
{
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// Repersents the BootStrap badge that appears on the userpanel.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Indicates if the account has access to the admin panel.
        /// </summary>
        public bool IsAdministrator { get; set; }

        /// <summary>
        /// If a URL contains these specified key words then do not allow the user to access them.
        /// <br /> Stored as JSON.
        /// </summary>
        public string ExcludedURLComponents { get; set; }
    }
}