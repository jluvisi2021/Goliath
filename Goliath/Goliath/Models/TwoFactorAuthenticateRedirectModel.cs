using Goliath.Enums;

namespace Goliath.Models
{
    public class TwoFactorAuthenticateRedirectModel
    {
        public TwoFactorAction Action { get; set; }

        public bool TwoFactorCodeRequired { get; set; }

        public string TwoFactorCode { get; set; }
        public string Password { get; set; }

        public bool IsSubmitted { get; set; }
    }
}