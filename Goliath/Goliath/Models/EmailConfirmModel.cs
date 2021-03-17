using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Models
{
    public class EmailConfirmModel
    {
        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsEmailSent { get; set; }
    }
}
