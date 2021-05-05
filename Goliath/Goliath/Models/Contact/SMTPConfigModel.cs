namespace Goliath.Models
{
    /// <summary>
    /// Parses settings from appsettings.json to utilize in sending SMTP emails. <br /> If you are
    /// looking to setup a personal SMTP sender then view wiki docs for configuring appsettings.json.
    /// </summary>
    public class SMTPConfigModel
    {
        /// <summary>
        /// Example: test@example.com
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Example: 123HelloWorld!
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Example: Goliath
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Example: smtp.outlook.com
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Example: 587 -&gt; Outlook TLS
        /// </summary>
        public int Port { get; set; }
    }
}