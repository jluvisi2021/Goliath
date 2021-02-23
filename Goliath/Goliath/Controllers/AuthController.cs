using Goliath.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the Views for Authentication.
    /// </summary>
    public sealed class AuthController : Controller
    {
        public AuthController()
        {
        }

        public IActionResult Index()
        {
            return View("Login", new AuthModel
            {
                SelectedValue = Models.Enums.AuthPage.Login
            });
        }

        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register(SignUpUserModel model)
        {
            if(ModelState.IsValid)
            {
                ModelState.Clear();
            }
            return View();
        }
        public IActionResult RegisterMethod()
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