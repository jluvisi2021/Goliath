using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goliath.Data
{
    /// <summary>
    /// Unauthorized Timeouts are timeouts which are given to accounts for requests that are done by
    /// unauthorized users. <br /> For Example: requesting to resend a verification email should be
    /// delayed and should not be able to be done without restriction. <br /> This database table
    /// manages a NumericID, UserID, and then a variety of other columns which hold DateTime values
    /// of the last request. <br /><em> All timestamps are in UTC. </em>
    /// </summary>
    [Table("UnauthorizedTimeoutTable")]
    public class UnauthorizedTimeouts
    {
        [Key]
        public int NumericID { get; set; }

        [Required]
        [MaxLength(36)]
        public string UserId { get; set; }

        [MaxLength(22)]
        public string RequestVerifyEmail { get; set; }

        [MaxLength(22)]
        public string RequestForgotPassword { get; set; }

        [MaxLength(22)]
        public string RequestForgotUsername { get; set; }

        [MaxLength(22)]
        public string RequestTwoFactorSmsInital { get; set; }

        [MaxLength(22)]
        public string RequestTwoFactorSmsResend { get; set; }
    }
}