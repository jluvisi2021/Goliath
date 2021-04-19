using Goliath.Models;

namespace Goliath.Services
{
    public interface ITwilioService
    {
        void SendSMS(SMSTextModel model);
    }
}