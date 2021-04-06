﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Models
{
    /// <summary>
    /// Used for the "Profile" tab in UserPanel.<br />
    /// Belongs to Partial View for modifying individual settings.
    /// </summary>
    public class ProfileSettingsGeneralModel
    {
        [Required]
        public string BackgroundColor { get; set; }
        [Required]
        public string DarkThemeEnabled { get; set; }
    }
}
