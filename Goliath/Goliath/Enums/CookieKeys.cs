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
    }
}