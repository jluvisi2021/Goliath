using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goliath.Data
{
    /// <summary>
    /// User Timeouts are timeouts which are given to accounts for requests that are done by
    /// unauthorized/authorized users. <br /> For Example: requesting to resend a verification email should be
    /// delayed and should not be able to be done without restriction. <br /> This database table
    /// manages a NumericId, UserID, and then a variety of other columns which hold DateTime values
    /// of the last request. <br /><em> All timestamps are in UTC. </em>
    /// </summary>
    [Table("UserTimeoutsTable")]
    public class UserTimeouts
    {
        [Key]
        public int NumericId { get; set; }

        [Required]
        [MinLength(36)]
        [MaxLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// Unauthorized Request: For when a user requests a new verification email
        /// for a new email.
        /// </summary>
        [MaxLength(22)]
        public string RequestVerifyEmail { get; set; }

        /// <summary>
        /// Unauthorized Request: For then a user attempts to reset their password
        /// on the authentication screen.
        /// </summary>
        [MaxLength(22)]
        public string RequestForgotPassword { get; set; }

        /// <summary>
        /// Unauthorized Request:
        /// For when a user requests to find their username.
        /// </summary>
        [MaxLength(22)]
        public string RequestForgotUsername { get; set; }

        /// <summary>
        /// Unauthorized Request: For when a user logins in and is sent to the two-factor screen but has
        /// not yet requested a resend.
        /// </summary>
        [MaxLength(22)]
        public string RequestTwoFactorSmsInital { get; set; }

        /// <summary>
        /// Unauthorized Request: For when a user requests an SMS resend and they are not authenticated.
        /// </summary>
        [MaxLength(22)]
        public string RequestTwoFactorSmsResend { get; set; }

        /// <summary>
        /// Authorized Request: For when a user requests to download their data.
        /// </summary>
        [MaxLength(22)]
        public string RequestDataDownload { get; set; }

        /// <summary>
        /// Authorized Request: For when a user requests an SMS two-factor code.
        /// </summary>
        [MaxLength(22)]
        public string RequestTwoFactorSmsAuthorized { get; set; }

        /// <summary>
        /// Authorized Request: For when a user updates their profile settings.
        /// </summary>
        [MaxLength(22)]
        public string UpdateProfileSettings { get; set; }
    }
}