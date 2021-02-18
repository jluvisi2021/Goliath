
using Goliath.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public AuthPage SelectedValue { get; set; }
        
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
