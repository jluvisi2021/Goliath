using Goliath.Models;
using System.Threading.Tasks;

namespace Goliath.Services
{
    public interface ITwilioService
    {
        Task<bool> SendSmsAsync(SMSTextModel model);
    }
}