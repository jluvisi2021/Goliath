using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goliath.Data
{
    /// <summary>
    /// Tokens which have been generated and prove the user is a human.
    /// </summary>
    [Table("ValidCaptchaCookieTokens")]
    public class ValidHumanVerifyTokens
    {
        [Key]
        public int NumericID { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string GeneratedDateTime { get; set; }
    }
}