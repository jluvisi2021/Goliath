
using Goliath.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goliath.Controllers
{

    /// <summary>
    /// Manages the Views for the Home.
    /// </summary>
    public sealed class HomeController : Controller
    {

        public HomeController()
        {

        }
        
        public IActionResult Index()
        {   
            return View("Login", new AuthModel
            {
                SelectedValue = Models.Enums.AuthPage.Login
            });
        }

        public IActionResult Register()
        {
            return View(new AuthModel
            {
                SelectedValue = Models.Enums.AuthPage.Register
            });
        }

        public IActionResult ForgotPassword()
        {
            return View(new AuthModel
            {
                SelectedValue = Models.Enums.AuthPage.Forgot_Password
            });
        }

        public IActionResult VerifyEmail()
        {
            return View(new AuthModel
            {
                SelectedValue = Models.Enums.AuthPage.Verify_Email
            });
        }
    }
}
