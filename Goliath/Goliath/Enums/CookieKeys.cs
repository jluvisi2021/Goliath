namespace Goliath.Enums
{
    /// <summary>
    /// The individual names of the keys of cookies made by Goliath.
    /// </summary>
    public static class CookieKeys
    {
        public const string AuthenticationCookie = "GoliathAuthenticate";
        public const string AntiForgeryCookie = "AntiForgeryCSRF";
        public const string ValidateCaptchaCookie = "CaptchaValid";
        public const string TwoFactorAuthorizeCookie = "TwoFactorAuthorize";
        public const string AspNetSessionCookie = "Session";
        public const string IdentityTwoFactorCookie = "TwoFactorIdentity";
        public const string IdentityTwoFactorRememberMeCookie = "TwoFactorIdentityStore";
        public const string AspNetTempDataCookie = "GoliathTemporary";
    }
}