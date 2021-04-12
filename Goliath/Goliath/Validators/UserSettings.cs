using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Goliath.Validators
{
    /// <summary>
    /// Validates the custom Goliath settings which can be<br />
    /// configured in the UserPanel.
    /// </summary>
    public class UserSettings : ValidationAttribute
    {
        /// <summary>
        /// The type of setting model to validate.
        /// Ex. General, Colors, etc
        /// </summary>
        private SettingsType _settingsValidator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">The type of settings model to validate.</param>
        public UserSettings(SettingsType type)
        {
            _settingsValidator = type;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if(_settingsValidator.Equals(SettingsType.General))
            {
                var settings = (ProfileSettingsGeneralModel)validationContext.ObjectInstance;
                if(!BackgroundColorValid(settings.BackgroundColor))
                {
                    return new ValidationResult("Background Color: Invalid");
                }
                if(string.IsNullOrWhiteSpace(settings.DarkThemeEnabled))
                {
                    return new ValidationResult("Dark Theme Enabled: Invalid");
                }
                if(!EmailValid(settings.NewEmail))
                {
                    return new ValidationResult("Email Address: Invalid");
                }
                return ValidationResult.Success;
            }
            
            return ValidationResult.Success;
        }

        
        private static bool EmailValid(string email)
        {
            return string.IsNullOrWhiteSpace(email) || (Regex.IsMatch(email, @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
+ "@"
+ @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$") && email.Length <= 40 && email.Length >= 3);
        }

        /// <param name="value">Value to be checked.</param>
        /// <returns>If the value passed is a valid hex color.</returns>
        private static bool BackgroundColorValid(string value)
        {
            if(!ValidString(value))
            {
                return false;
            }
            if(value.Length != 7)
            {
                return false;
            }
            if(value[0] != '#')
            {
                return false;
            }
            int res = 0;
            return int.TryParse(value[1..], System.Globalization.NumberStyles.HexNumber,
         System.Globalization.CultureInfo.InvariantCulture, out res);
        }

        private static bool ValidString(string val)
        {
            if(string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val))
            {
                return false;
            }
            return true;
        }
    }
}
