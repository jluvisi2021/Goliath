using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goliath.Data
{
    /// <summary>
    /// The two factor authorize token is a token given when the user logs in with their user name
    /// and password but still needs to put their two-factor code in. <br /><br /> In order to
    /// prevent a random individual from simply copying the URL for two factor: Ex -&gt;
    /// "/account/validate?userName=GoliathAdmin" and then guessing the number token we can add a
    /// secure layer on top of it. <br /><br /> This table stores UserId's as well as a random
    /// number. Once the user authenticates with two-factor the random number is removed from the
    /// database. In order to input the two-factor code in the first place the random token must be
    /// valid and present.
    /// </summary>
    [Table("TwoFactorAuthorizationTokens")]
    public class TwoFactorAuthorizeToken
    {
        [Key]
        public int NumericID { get; set; }

        [MaxLength(36)]
        public string UserId { get; set; }

        public string AuthorizeToken { get; set; }
    }
}