using Goliath.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Goliath.Services
{
    /// <inheritdoc cref="ITwilioService" />
    public class TwilioService : ITwilioService
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly string _accountSid;
        private readonly string _authToken;

        public TwilioService(IConfiguration config, ILogger<TwilioService> logger)
        {
            _config = config;
            _logger = logger;
            _accountSid = _config["Twilio:AccountSID"];
            _authToken = _config["Twilio:AuthToken"];
        }

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
                _logger.LogInformation($"Sent an SMS message to {model.To} from {model.From}.");
            }
            catch (System.Exception)
            {
                _logger.LogError($"Failed to send an SMS message to {model.To} using phone number {model.From}.");
                return false;
            }
            return true;
        }
    }
}