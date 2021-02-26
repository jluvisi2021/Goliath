using Goliath.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the Views for Authentication.
    /// ButtonID indicates the radio button to be checked.
    /// </summary>

    public sealed class AuthController : Controller
    {
        public AuthController()
        {
        }

        public IActionResult Index()
        {
            ViewData["ButtonID"] = "login";
            return View("Login");
        }

        [Route("register/goliath")]
        public IActionResult Register()
        {
            ViewData["ButtonID"] = "register";
            return View();
        }

        [Route("register/goliath")]
        [HttpPost]
        public IActionResult Register(SignUpUserModel model)
        {
            ViewData["ButtonID"] = "register";
            if (ModelState.IsValid)
            {
                ModelState.Clear();
            }
            return View();
        }

        [Route("register/method")]
        public IActionResult RegisterMethod()
        {
            ViewData["ButtonID"] = "register";
            return View();
        }

        [Route("forgotpassword")]
        public IActionResult ForgotPassword()
        {
            ViewData["ButtonID"] = "forgot-password";
            return View();
        }

        [Route("verify")]
        public IActionResult VerifyEmail()
        {
            ViewData["ButtonID"] = "verify-email";
            return View();
        }
    }
}