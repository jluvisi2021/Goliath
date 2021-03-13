using Goliath.Models;
using System.Threading.Tasks;

namespace Goliath.Services
{
    public interface IEmailService
    {
        Task<bool> SendTestEmail(UserEmailOptions options);
        Task<bool> SendConfirmationEmail(UserEmailOptions options);
    }
}