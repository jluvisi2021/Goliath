using Goliath.Models;
using System.Threading.Tasks;

namespace Goliath.Services
{
    public interface IEmailService
    {
        Task<bool> SendTestEmail(UserEmailOptions options);

        Task<bool> SendConfirmationEmail(UserEmailOptions options);

        Task<bool> ResendConfirmationEmail(UserEmailOptions options);

        Task<bool> SendForgotPasswordEmail(UserEmailOptions options);
    }
}