using Goliath.Validators;
using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    /// <summary>
    /// Used for the "Profile" tab in UserPanel. <br /> Belongs to Partial View for modifying
    /// individual settings.
    /// </summary>
    [UserSettings(Enums.SettingsType.General)]
    public class ProfileSettingsGeneralModel
    {
        public string BackgroundColor { get; set; }

        public string DarkThemeEnabled { get; set; }

        public string CurrentEmail { get; set; }
        public string NewEmail { get; set; }

        public string CurrentPhoneNumber { get; set; }
        public string NewPhoneNumber { get; set; }

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}