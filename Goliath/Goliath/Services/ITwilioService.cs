using Goliath.Models;
using System.Threading.Tasks;

namespace Goliath.Services
{
    /// <summary>
    /// A class which is used for interacting with Twilio.
    /// </summary>
    public interface ITwilioService
    {
        /// <summary>
        /// Sends an SMS text message to a specified user with a specified message.
        /// </summary>
        /// <param name="model"> The text and phone number to send to. </param>
        /// <returns> If Twilio was successfully able to send the message. </returns>
        Task<bool> SendSmsAsync(SMSTextModel model);
    }
}