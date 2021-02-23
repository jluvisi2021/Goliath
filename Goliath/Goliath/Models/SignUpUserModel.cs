using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Models
{
    public class SignUpUserModel
    {
        [Required(ErrorMessage = "<strong>Please enter a username.</strong>")]
        [DataType(DataType.Text)]
        [Display(Name ="Username")]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "Username must be between 8 and 16 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        [Compare("ConfirmEmail", ErrorMessage = "Email does not match.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please confirm your email.")]
        [EmailAddress(ErrorMessage = "Please confirm your email.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm Email")]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [Compare("ConfirmPassword", ErrorMessage ="Password does not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
