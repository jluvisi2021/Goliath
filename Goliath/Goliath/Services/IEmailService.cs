using Goliath.Models;
using System.Threading.Tasks;

namespace Goliath.Services
{
    public interface IEmailService
    {
        Task<bool> SendTestEmailAsync(UserEmailOptions options);

        Task<bool> SendConfirmationEmailAsync(UserEmailOptions options);

        Task<bool> ResendConfirmationEmailAsync(UserEmailOptions options);

        Task<bool> SendForgotPasswordEmailAsync(UserEmailOptions options);

        Task<bool> SendForgotUsernameEmailAsync(UserEmailOptions options);

        Task<bool> SendRoleMovedEmailAsync(UserEmailOptions options);

        Task<bool> SendConfirmNewEmailAsync(UserEmailOptions options);

        Task<bool> SendNotifyOldEmailAsync(UserEmailOptions options);

        Task<bool> SendVerifyPhoneEmailAsync(UserEmailOptions options);
    }
}