using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Models.Accounts
{
    public class DeleteAccountModel
    {
        [Required(ErrorMessage = "Please enter your username.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string TwoFactorCode { get; set; }

        [MaxLength(300)]
        public string Feedback { get; set; }
    }
}
