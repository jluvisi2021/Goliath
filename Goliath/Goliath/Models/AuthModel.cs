using Goliath.Models.Enums;
using System;

namespace Goliath.Models
{
    /*
    public enum AuthPage
    {
        Login, Register, Forgot_Password, Verify_Email
    }
    */

    public class AuthModel
    {
        /// <summary>
        /// Get/Set the current value of the selected button in the authentication window.
        /// Ex. Register -> Color Register Button Blue
        /// </summary>
        public AuthPage SelectedValue { get; set; }

        /// <summary>
        /// Returns the Enum value <see cref="AuthPage.enums.cs"/>
        /// from a string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public AuthPage FromString(string str)
        {
            return str switch
            {
                "Login" => AuthPage.Login,
                "Register" => AuthPage.Register,
                "Forgot_Password" => AuthPage.Forgot_Password,
                "Verify_Email" => AuthPage.Verify_Email,
                _ => throw new NullReferenceException(),
            };
        }

        /// <summary>
        /// Convert current select value item to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SelectedValue switch
            {
                AuthPage.Login => "Login",
                AuthPage.Register => "Register",
                AuthPage.Forgot_Password => "Forgot_Password",
                AuthPage.Verify_Email => "Verify_Email",
                _ => throw new NullReferenceException(),
            };
        }
    }
}