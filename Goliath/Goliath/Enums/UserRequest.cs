namespace Goliath.Enums
{
    /// <summary>
    /// Enums which represent different requests that can be made by anonymous or authorized users
    /// according to the <see cref="Data.UserTimeouts"/>.
    /// </summary>
    public enum UserRequest
    {
        RequestVerificationEmail,
        RequestUsernameEmail,
        RequestForgotPasswordEmail,
        InitalTwoFactorRequestSms,
        RequestTwoFactorResendSms,
        RequestDataDownload,
        RequestTwoFactorSmsAuthorized,
        UpdateProfileSettings
    }
}