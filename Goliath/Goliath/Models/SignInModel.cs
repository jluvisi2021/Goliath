using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    public class SignInModel
    {
        [Required]
        [Display(Name = "Username")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}