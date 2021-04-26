﻿using Goliath.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
                if (string.IsNullOrWhiteSpace(settings.BackgroundColor))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult("Input a valid background color.");
            }
            if (string.IsNullOrWhiteSpace(settings.DarkThemeEnabled))
            {
                return new ValidationResult("Invalid theme selection.");
            }

            return ValidationResult.Success;
        }

        private static bool EmailValid(string email)
        {
            return string.IsNullOrWhiteSpace(email) || (Regex.IsMatch(email, @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
+ "@"
+ @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$") && email.Length <= 40 && email.Length >= 3);
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