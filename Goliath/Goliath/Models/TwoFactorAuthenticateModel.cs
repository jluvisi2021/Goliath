using Goliath.Enums;

namespace Goliath.Models
{
    public class TwoFactorAuthenticateModel
    {
        public string InputUsername { get; set; }
        public string InputTwoFactorCode { get; set; }
        public bool IsRecoveryCode { get; set; }
        public bool RememberMe { get; set; }
        public TwoFactorMethod UserMethod { get; set; }
    }
}