using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Enums
{
    /// <summary>
    /// Enums which represent different requests that can be made by
    /// anonymous users.
    /// </summary>
    public enum UnauthorizedRequest
    {
        RequestVerificationEmail,
        RequestUsernameEmail,
        RequestForgotPasswordEmail,
        /// <summary>
        /// A request which represents a user attempting to login with two factor
        /// through SMS the first time. (Not a resend)
        /// </summary>
        InitalTwoFactorRequestSms,
        RequestTwoFactorResendSms
    }
}
