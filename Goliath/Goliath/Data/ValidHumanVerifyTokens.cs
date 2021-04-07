using System.ComponentModel.DataAnnotations;

namespace Goliath.Data
{
    /// <summary>
    /// Tokens which have been generated and prove the user is a human.
    /// </summary>
    public class ValidHumanVerifyTokens
    {
        [Key]
        public int NumericID { get; set; }

        public string Token { get; set; }
        public string GeneratedDateTime { get; set; }
    }
}