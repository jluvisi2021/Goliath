﻿using Goliath.Models;
using Goliath.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the Views for Authentication.
    /// ButtonID indicates the radio button to be checked.
    /// </summary>

    public sealed class AuthController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AuthController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // Refereed to as "Login" as well.
        public IActionResult Index()
        {
            ViewData["ButtonID"] = "login";
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInModel signInModel)
        {
            ViewData["ButtonID"] = "login";
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("Received Model.");
                System.Diagnostics.Debug.WriteLine("Username: " + signInModel.Username);
                var result = await _accountRepository.PasswordSignInAsync(signInModel);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "UserPanel");
                }

                ModelState.AddModelError("", "Invalid Credentials.");
            }
            return View("Login", signInModel);
        }

        [Route("register/goliath")]
        public IActionResult Register()
        {
            ViewData["ButtonID"] = "register";
            return View();
        }

        [Route("register/goliath")]
        [HttpPost]
        public async Task<IActionResult> Register(SignUpUserModel model)
        {
            ViewData["ButtonID"] = "register";
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.CreateUserAsync(model);
                if (!result.Succeeded)
                {
                    foreach (var errorMessage in result.Errors)
                    {
                        ModelState.AddModelError("", errorMessage.Description);
                    }

                    return View(model);
                }
                ModelState.Clear();
            }
            // Send the user back to the login screen with a message.
            TempData["Redirect"] = true;
            return RedirectToAction("Index");
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