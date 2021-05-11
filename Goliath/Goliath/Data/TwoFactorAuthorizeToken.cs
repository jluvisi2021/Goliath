using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goliath.Data
{
    /// <summary>
    /// <para>
    /// The two factor authorize token is a token given when the user logs in with their user name
    /// and password but still needs to put their two-factor code in.
    /// </para>
    /// <para>
    /// In order to prevent a random individual from simply copying the URL for two factor: Ex -&gt;
    /// <c> "/account/validate?userName=GoliathAdmin" </c> and then guessing the number token we can
    /// add a secure layer on top of it.
    /// </para>
    /// <para>
    /// This table stores UserId's as well as a random number. Once the user authenticates with
    /// two-factor the random number is removed from the database. In order to input the two-factor
    /// code in the first place the random token must be valid and present.
    /// </para>
    /// Tokens hashed with: <see cref="Goliath.Helper.GoliathHash" />
    /// </summary>
    [Table("TwoFactorAuthorizationTokens")]
    public class TwoFactorAuthorizeToken
    {
        [Key]
        public int NumericId { get; set; }

        [Required]
        [MinLength(36)]
        [MaxLength(36)]
        public string UserId { get; set; }

        [Required]
        [MinLength(Helper.GoliathHelper.MinimumTokenValueLength)]
        [MaxLength(Helper.GoliathHelper.MaximumTokenValueLength)]
        public string AuthorizeToken { get; set; }
    }
}