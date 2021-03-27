namespace Goliath.Enums
{
    /// <summary>
    /// Manages the purpose for redirection to different pages. <br /> All redirection data should
    /// be stored in TempData[enum]
    /// </summary>
    public enum RedirectPurpose
    {
        RegisterSuccess,
        LogoutSuccess,
        VerifiedEmailSuccess,
        VerifiedEmailFailure,
        ResetPasswordSuccess,
        ResetPasswordFailure,
        ResetPasswordModal
    }
}