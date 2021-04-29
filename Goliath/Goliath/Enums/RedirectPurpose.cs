namespace Goliath.Enums
{
    /// <summary>
    /// Tells the purpose for redirection to different pages. <br /> All redirection data should be
    /// stored in TempData[enum]
    /// </summary>
    public enum RedirectPurpose
    {
        None,

        // Authentication Panel
        RegisterSuccess,

        LogoutSuccess,
        VerifiedEmailSuccess,
        VerifiedEmailFailure,
        ResetPasswordSuccess,
        ResetPasswordFailure,
        ResetPasswordModal,
        TwoFactorSmsResendSuccess,
        TwoFactorSmsResendFailureTimeout,
        TwoFactorSmsResendFailure,
        Exception,

        // User Panel
        SettingsUpdatedSuccess,

        TwoFactorEnabled,
        TwoFactorDisabled,
        VerificationCodes
    }
}