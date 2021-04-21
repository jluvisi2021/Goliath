using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goliath.Data
{
    /// <summary>
    /// Holds UserID's and a date time. If the UserID is not in the database or the date time is
    /// over 2 hours old then it allows a user to request a new verification code for their phone.
    /// </summary>
    [Table("ResendSmsConfirmationToken")]
    public class ResendSmsConfirmationToken
    {
        [Key]
        public int NumericID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        [MaxLength(22)]
        public string TokenSentTimestamp { get; set; }
    }
}