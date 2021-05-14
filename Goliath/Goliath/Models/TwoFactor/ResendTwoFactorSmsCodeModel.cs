namespace Goliath.Models
{
    public class ResendTwoFactorSmsCodeModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }
        public bool IsUrnRedirect { get; set; }

        /// <summary>
        /// Use the ReturnPath when you need to redirect to a local URL with query parameters.
        /// IsUrnRedirect must also be true to read the returnPath.
        /// </summary>
        public string ReturnPath { get; set; }
        public bool? UtilizeHtmlMessage { get; set; } = true;
    }
}