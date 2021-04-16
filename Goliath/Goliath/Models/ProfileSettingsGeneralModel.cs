﻿using Goliath.Validators;
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

        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        public string NewEmail { get; set; }

        public string CurrentPhoneNumber { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string NewPhoneNumber { get; set; }

        [DataType(DataType.Password)]
        [StringLength(2048, MinimumLength = 6, ErrorMessage = "Password length must be 6-2048 characters.")]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "Password cannot contain spaces.")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("ConfirmNewPassword", ErrorMessage = "Please confirm your password.")]
        [StringLength(2048, MinimumLength = 6, ErrorMessage = "Password length must be 6-2048 characters.")]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "Password cannot contain spaces.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }

        [StringLength(2, MinimumLength = 1, ErrorMessage = "The logout threshold must be between 0 and 99 minutes.")]
        public string LogoutThreshold { get; set; }
    }
}