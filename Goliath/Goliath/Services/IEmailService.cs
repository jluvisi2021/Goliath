using Goliath.Models;
using System.Threading.Tasks;

namespace Goliath.Services
{
    /// <summary>
    /// A class which is used for sending emails to users over the <b> SMTP Protocol </b>.
    /// </summary>
    public interface IEmailService
    {
        Task<bool> SendTestEmailAsync(UserEmailOptions options);

        /// <summary>
        /// Sends an email to a user to confirm their email address. <br /> This email is only every
        /// sent once to any user.
        /// </summary>
        /// <param name="options"> The email's options from <see cref="UserEmailOptions" />. </param>
        /// <returns> If the email was sent successfully. </returns>
        Task<bool> SendConfirmationEmailAsync(UserEmailOptions options);

        /// <summary>
        /// Sends an email to a user to confirm their email address.
        /// </summary>
        /// <param name="options"> The email's options from <see cref="UserEmailOptions" />. </param>
        /// <returns> If the email was sent successfully. </returns>
        Task<bool> ResendConfirmationEmailAsync(UserEmailOptions options);

        /// <summary>
        /// Sends an email to a user to confirm a password reset.
        /// </summary>
        /// <param name="options"> The email's options from <see cref="UserEmailOptions" />. </param>
        /// <returns> If the email was sent successfully. </returns>
        Task<bool> SendForgotPasswordEmailAsync(UserEmailOptions options);

        /// <summary>
        /// Sends an email to a user with their respective username.
        /// </summary>
        /// <param name="options"> The email's options from <see cref="UserEmailOptions" />. </param>
        /// <returns> If the email was sent successfully. </returns>
        Task<bool> SendForgotUsernameEmailAsync(UserEmailOptions options);

        /// <summary>
        /// Sends an email to a user notifying them that their <see cref="ApplicationRole" /> has
        /// been changed.
        /// </summary>
        /// <param name="options"> The email's options from <see cref="UserEmailOptions" />. </param>
        /// <returns> If the email was sent successfully. </returns>
        Task<bool> SendRoleMovedEmailAsync(UserEmailOptions options);

        /// <summary>
        /// Sends an email to the user with a key to decrypt their data.
        /// </summary>
        /// <param name="options"> The email's options from <see cref="UserEmailOptions" />. </param>
        /// <returns> If the email was sent successfully. </returns>
        Task<bool> SendDataEncryptionEmailAsync(UserEmailOptions options);

        /// <summary>
        /// Sends a confirmation email to a user's new email address.
        /// </summary>
        /// <param name="options"> The email's options from <see cref="UserEmailOptions" />. </param>
        /// <returns> If the email was sent successfully. </returns>
        Task<bool> SendConfirmNewEmailAsync(UserEmailOptions options);

        /// <summary>
        /// Sends an email to a user notifying them that their email address has been changed.
        /// </summary>
        /// <param name="options"> The email's options from <see cref="UserEmailOptions" />. </param>
        /// <returns> If the email was sent successfully. </returns>
        Task<bool> SendNotifyOldEmailAsync(UserEmailOptions options);

        /// <summary>
        /// Sends an email to a user notifying them that their phone number was changed.
        /// </summary>
        /// <param name="options"> The email's options from <see cref="UserEmailOptions" />. </param>
        /// <returns> If the email was sent successfully. </returns>
        Task<bool> SendVerifyPhoneEmailAsync(UserEmailOptions options);
    }
}