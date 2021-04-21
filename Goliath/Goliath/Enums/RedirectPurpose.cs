namespace Goliath.Enums
{
    /// <summary>
    /// Tells the purpose for redirection to different pages. <br /> All redirection data should be
    /// stored in TempData[enum]
    /// </summary>
    public enum RedirectPurpose
    {
        // Authentication Panel
        RegisterSuccess,

        LogoutSuccess,
        VerifiedEmailSuccess,
        VerifiedEmailFailure,
        ResetPasswordSuccess,
        ResetPasswordFailure,
        ResetPasswordModal,
        Exception,

        // User Panel
        SettingsUpdatedSuccess
    }
}