using Goliath.Models;
using Microsoft.Extensions.Configuration;
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

        public void SendSMS(SMSTextModel model)
        {
            if (string.IsNullOrWhiteSpace(model.From))
            {
                model.From = _config["Twilio:DefaultFrom"];
            }
            TwilioClient.Init(_accountSid, _authToken);
            _ = MessageResource.Create(
                body: model.Message,
                from: new Twilio.Types.PhoneNumber(model.From),
                to: new Twilio.Types.PhoneNumber(model.To)
            );
        }
    }
}