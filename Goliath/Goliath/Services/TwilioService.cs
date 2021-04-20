using Goliath.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Goliath.Services
{
    public class TwilioService : ITwilioService
    {
        private readonly IConfiguration _config;
        private readonly string _accountSid;
        private readonly string _authToken;

        public TwilioService(IConfiguration config)
        {
            _config = config;

            _accountSid = _config["Twilio:AccountSID"];
            _authToken = _config["Twilio:AuthToken"];
        }

        /// <summary>
        /// Sends an Async message to an SMS using Twilio SMS service.
        /// </summary>
        /// <param name="model"> Data for the message. </param>
        /// <returns> If the message was sent. </returns>
        public async Task<bool> SendSmsAsync(SMSTextModel model)
        {
            if (string.IsNullOrWhiteSpace(model.From))
            {
                model.From = _config["Twilio:DefaultFrom"];
            }
            TwilioClient.Init(_accountSid, _authToken);
            try
            {
                await MessageResource.CreateAsync
                    (
                body:
                    model.Message,
                from:
                    new Twilio.Types.PhoneNumber(model.From),
                to:
                    new Twilio.Types.PhoneNumber(model.To)
                    );
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }
    }
}