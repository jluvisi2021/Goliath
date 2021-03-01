using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string BackgroundColor { get; set; }

        public string DarkTheme { get; set; }

        /// <summary>
        /// Manages all of the password data that the user has.
        /// </summary>
        public string UserData { get; set; }


    }
}
