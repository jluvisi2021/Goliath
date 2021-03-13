using Goliath.Models;
using Goliath.Repository;
using Goliath.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.InteropServices;
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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        public AuthController(IAccountRepository accountRepository, SignInManager<ApplicationUser> signInManager, IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        // Refereed to as "Login" as well.
        public IActionResult Index()
        {
            // If the user is signed in redirect them to the user panel.
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "UserPanel");
            }
            ViewData["ButtonID"] = "login";
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInModel signInModel)
        {
            ViewData["ButtonID"] = "login";
            // If the user has signed in with valid data.
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.PasswordSignInAsync(signInModel);
                // If the user name and password match.
                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "UserPanel");
                }else if(result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Please try again later.");
                }else if(result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "You must verify your email!");
                    
                }else if(result.RequiresTwoFactor)
                {
                    ModelState.AddModelError("", "You must login through 2FA.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Credentials.");
                }
                
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
                // Pass in the information for the confirmation email.
                var result = await _accountRepository.CreateUserAsync(model, new string[] { Request.Headers["User-Agent"].ToString(), HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() });
                if (!result.Succeeded)
                {
                    foreach (var errorMessage in result.Errors)
                    {
                        ModelState.AddModelError("", errorMessage.Description);
                    }

                    return View(model);
                }
                
                // Registration is Valid.
                ModelState.Clear();
                // Send the user to the index with register tempdata.
                TempData["Redirect"] = "register";
                return RedirectToAction("Index");
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

        [Route("forgotusername")]
        public IActionResult ForgotUsername()
        {
            ViewData["ButtonID"] = "forgot-username";
            return View();
        }

        [Route("verify")]
        public IActionResult VerifyEmail()
        {
            ViewData["ButtonID"] = "verify-email";
            return View();
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string uid, string token)
        {

            // If the unique ID as well as the user exist.
            if(!string.IsNullOrWhiteSpace(uid) && !string.IsNullOrWhiteSpace(token))
            {
                token = token.Replace(" ", "+");
                // Check to make sure the token has not expired or is invalid.
               var result = await _accountRepository.ConfirmEmailAsync(uid, token);
                if(result.Succeeded)
                {
                    // If the email confirmation is a success then we can pass that info into the view.
                    TempData["Redirect"] = "verified";
                    return RedirectToAction("Index");
                }
            }
            
            return RedirectToAction("Index");
        }
    }
}