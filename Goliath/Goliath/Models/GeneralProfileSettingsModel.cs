using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Models
{
    public class GeneralProfileSettingsModel
    {
        public string BackgroundColor { get; set; }
        public string DarkTheme { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        public string CurrentEmail { get; set; }

        public string NewEmail { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string AccountCreationDate { get; set; }

        public string TimesLoggedIn { get; set; }

    }
}
