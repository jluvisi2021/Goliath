using Goliath.Models;
using System.ComponentModel.DataAnnotations;

namespace Goliath.Validators
{
    /// <summary>
    /// Validates the custom Goliath settings which can be <br /> configured in the UserPanel.
    /// </summary>
    public class UserSettingsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ProfileSettingsModel settings = (ProfileSettingsModel)validationContext.ObjectInstance;
            if (!BackgroundColorValid(settings.BackgroundColor))
            {
                return new ValidationResult("Input a valid background color.");
            }
            if (string.IsNullOrWhiteSpace(settings.DarkThemeEnabled))
            {
                return new ValidationResult("Invalid theme selection.");
            }
            if (string.IsNullOrWhiteSpace(settings.LogoutThreshold))
            {
                return new ValidationResult("Logout threshold must be a number.");
            }
            if (!int.TryParse(settings.LogoutThreshold, out int a))
            {
                return new ValidationResult("Logout threshold must be a number.");
            }
            else
            {
                if (a <= 0)
                {
                    return new ValidationResult("Logout threshold must be greater than one.");
                }
            }
            return ValidationResult.Success;
        }

        /// <param name="value"> Value to be checked. </param>
        /// <returns> If the value passed is a valid hex color. </returns>
        private static bool BackgroundColorValid(string value)
        {
            if (!ValidString(value))
            {
                return false;
            }
            if (value.Length != 7)
            {
                return false;
            }
            if (value[0] != '#')
            {
                return false;
            }
            return int.TryParse(value[1..], System.Globalization.NumberStyles.HexNumber,
         System.Globalization.CultureInfo.InvariantCulture, out int res);
        }

        private static bool ValidString(string val)
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val))
            {
                return false;
            }
            return true;
        }
    }
}