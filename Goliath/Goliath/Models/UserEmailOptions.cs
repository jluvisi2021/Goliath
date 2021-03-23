using System.Collections.Generic;

namespace Goliath.Models
{
    /// <summary>
    /// Options when sending emails through the email service.
    /// </summary>
    public class UserEmailOptions
    {
        /// <summary>
        /// All the emails that the SMTP server will send to.
        /// </summary>
        public List<string> ToEmails { get; set; }

        /// <summary>
        /// The subject of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Body of the email. Uses HTML.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// All of the individual placeholders for the email.<br/>
        /// Placeholders are denoted by {{NAME}}<br/>
        /// </summary>
        public Dictionary<string, string> Placeholders { get; set; }
    }
}