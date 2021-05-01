namespace Goliath.Enums
{
    /// <summary>
    /// Constant values which represent the keys that can be passed through TempData. All TempData
    /// in Goliath uses TempDataKeys and not magic strings.
    /// </summary>
    public static class TempDataKeys
    {
        /// <summary>
        /// For Redirecting the user. Should ALWAYS be used with <b> "RedirectPurpose" </b> enum.
        /// </summary>
        public const string Redirect = "Redirect";

        public const string ErrorInformation = "ErrorInformation";

        /// <summary>
        /// A TempData key which stores the value of an HTML string. <br /> When a view is displayed
        /// that supports HtmlMessage then it directly displays the value of HtmlMessage onto the
        /// view. <br /><b> Note: HTML passed through should always be encoded using <em>
        /// HttpUtility </em>. </b>
        /// </summary>
        public const string HtmlMessage = "HtmlMessage";

        /// <summary>
        /// Represents a TempData value which stores TwoFactor recovery codes. <br /> Should only be
        /// used in TwoFactor controller and views.
        /// </summary>
        public const string TwoFactorRecoveryCodes = "RecoveryCodes";

        public const string ModelErrors = "Errors";
        public const string Model = "Model";
    }
}