using DNTCaptcha.Core;
using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using Goliath.Repository;
using Goliath.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the Views for Authentication.
    /// <br />
    /// ButtonID indicates the radio button to be checked.
    ///
    /// </summary>
    public sealed class AuthController : Controller
    {
        /// <summary>
        /// For interfacing with the application user.
        /// </summary>
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        /// Manage if the user is signed in as well as authentication.
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Send emails.
        /// </summary>
        private readonly IEmailService _emailService;

        private readonly IDNTCaptchaValidatorService _validatorService;
        private readonly DNTCaptchaOptions _captchaOptions;

        public AuthController
            (
            IAccountRepository accountRepository,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IDNTCaptchaValidatorService validatorService,
            IOptions<DNTCaptchaOptions> options
            )
        {
            _accountRepository = accountRepository;
            _signInManager = signInManager;
            _emailService = emailService;
            _validatorService = validatorService;
            _captchaOptions = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;
        }

        // Referred to as "Login" as well.
        public IActionResult Index()
        {
            // If the user is signed in redirect them to the user panel.
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "UserPanel");
            }

            return RedirectToAction("Login");
        }

        [Route("login")]
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["ButtonID"] = ButtonID.Login;
            return View();
        }

        [Route("login")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(SignInModel signInModel)
        {
            ViewData["ButtonID"] = ButtonID.Login;

            // If the user has signed in with valid data.
            if (ModelState.IsValid)
            {
                if (!_validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.ShowDigits))
                {
                    ModelState.AddModelError(_captchaOptions.CaptchaComponent.CaptchaInputName, "Incorrect CAPTCHA.");
                    return View(signInModel);
                }

                Microsoft.AspNetCore.Identity.SignInResult result = await _accountRepository.PasswordSignInAsync(signInModel);
                // If the user name and password match.
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "UserPanel");
                    // Else we send the specified errors to the user.
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Please try again later.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "You must verify your email!");
                }
                else if (result.RequiresTwoFactor)
                {
                    ModelState.AddModelError(string.Empty, "You must login through 2FA.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Credentials.");
                }
            }
            return View(signInModel);
        }

        [Route("register/goliath")]
        public IActionResult Register()
        {
            ViewData["ButtonID"] = ButtonID.Register;
            return View();
        }

        [Route("register/goliath")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(SignUpUserModel model)
        {
            ViewData["ButtonID"] = ButtonID.Register;
            if (ModelState.IsValid)
            {
                // Pass in the information for the confirmation email.// new string[] { Request.Headers["User-Agent"].ToString(), HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()
                IdentityResult result = await _accountRepository.CreateUserAsync(model,
                new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));

                if (!result.Succeeded)
                {
                    // For every error that is created during registration we add that to the eventual bootstrap modal.
                    foreach (IdentityError errorMessage in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, errorMessage.Description);
                    }

                    return View(model);
                }

                // Registration is Valid.
                ModelState.Clear();
                // Send the user to the index with register tempdata.
                TempData["Redirect"] = RedirectPurpose.RegisterSuccess;
                return RedirectToAction("Index");
            }
            return View();
        }

        [Route("register/method")]
        public IActionResult RegisterMethod()
        {
            ViewData["ButtonID"] = ButtonID.Register;
            return View();
        }

        [Route("credentials/password")]
        public IActionResult ForgotPassword()
        {
            ViewData["ButtonID"] = ButtonID.ForgotPassword;
            return View();
        }

        [Route("credentials/password")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            ViewData["ButtonID"] = ButtonID.ForgotPassword;
            // Verify that the user exists with the specified email.
            ApplicationUser user = await _accountRepository.FindByEmailAsync(model.Email);

            if (user != null)
            {
                // If the username does not match.
                if (!(user.UserName.Equals(model.Username)))
                {
                    ModelState.AddModelError(string.Empty, $"Invalid Username \"{model.Username}\" for {model.Email}");
                    return View();
                }
                // If the email is not confirmed
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Confirm your account before resetting your password.");
                    return View();
                }
                // Generate a token as well as a user agent.
                await _accountRepository.GenerateForgotPasswordToken(user, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
                // Indicate to the View that the email was sent.
                model.IsEmailSent = true;
                // Clear all fields.
                ModelState.Clear();

                return View(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"We could not find user: {model.Email}");
            }

            return View();
        }

        [Route("credentials/username")]
        public IActionResult ForgotUsername()
        {
            ViewData["ButtonID"] = ButtonID.ForgotUsername;
            return View();
        }

        [Route("credentials/username")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotUsername(ForgotUsernameModel model)
        {
            ViewData["ButtonID"] = ButtonID.ForgotUsername;
            ApplicationUser user = await _accountRepository.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Please confirm your account before doing this.");
                    return View(model);
                }
                await _accountRepository.GenerateUsername(user, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
                model.IsEmailSent = true;
                ModelState.Clear();
                return View(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"We could not find user: {model.Email}");
            }

            return View();
        }

        [Route("verify")]
        public IActionResult VerifyEmail()
        {
            ViewData["ButtonID"] = ButtonID.VerifyEmail;
            return View();
        }

        [Route("verify")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(EmailConfirmModel model)
        {
            ViewData["ButtonID"] = ButtonID.VerifyEmail;
            // Verify that the user exists with the specified email.
            ApplicationUser user = await _accountRepository.FindByEmailAsync(model.Email);

            if (user != null)
            {
                // If the email is already confirmed.
                if (user.EmailConfirmed)
                {
                    model.IsConfirmed = true;
                    ModelState.AddModelError(string.Empty, "Account already verified.");
                    return View(model);
                }
                // Generate a token as well as a user agent.
                await _accountRepository.GenerateEmailConfirmationToken(user, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));

                // Indicate to the View that the email was sent.
                model.IsEmailSent = true;
                // Clear all fields.
                ModelState.Clear();

                return View(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"We could not find user: {model.Email}");
            }

            return View();
        }

        /// <summary>
        /// Verifies an email given a UID and TOKEN.<br />
        /// Does not contain a view and redirects to index view.<br />
        /// The result of the method call is returned by the Bootstrap modal.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string uid, string token)
        {
            // If the unique ID as well as the user exist.
            if (!string.IsNullOrWhiteSpace(uid) && !string.IsNullOrWhiteSpace(token))
            {
                token = token.Replace(' ', '+');
                // Check to make sure the token has not expired or is invalid.
                IdentityResult result = await _accountRepository.ConfirmEmailAsync(uid, token);
                if (result.Succeeded)
                {
                    // If the email confirmation is a success then we can pass that info into the view.
                    TempData["Redirect"] = RedirectPurpose.VerifiedEmailSuccess;
                    return RedirectToAction("Index");
                }
            }
            // Alert to the view that the verification failed.
            TempData["Redirect"] = RedirectPurpose.VerifiedEmailFailure;
            return RedirectToAction("Index");
        }

        [HttpGet("forgot-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            ViewData["ButtonID"] = ButtonID.ForgotPassword;

            if(string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(token))
            {
                TempData["Redirect"] = RedirectPurpose.ResetPasswordFailure;
                return RedirectToAction("Index");
            }
            // Add the userID and token from the url.
            ResetPasswordModel model = new()
            {
                UserId = uid,
                Token = token
            };
            // Tell the view we are redirecting to make a new password.
            TempData["Redirect"] = RedirectPurpose.ResetPasswordModal;
            // Serialize the model and pass it.
            TempData["Model"] = JsonConvert.SerializeObject(model);
            return RedirectToAction("Index");
        }

        [HttpPost("forgot-password"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            ViewData["ButtonID"] = ButtonID.ForgotPassword;
            List<string> errors = new();
            if (ModelState.IsValid)
            {
                model.Token = model.Token.Replace(' ', '+');
                IdentityResult result = await _accountRepository.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsCompleted = true;
                    // Completed Successfully
                    TempData["Redirect"] = RedirectPurpose.ResetPasswordSuccess;
                    return RedirectToAction("Index");
                }

                // Go through all errors and add them to the view.
                foreach (IdentityError error in result.Errors)
                {
                    errors.Add(error.Description);
                }
            }
            foreach (KeyValuePair<string, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateEntry> error in ViewData.ModelState)
            {
                if (error.Value.Errors.Count > 0)
                {
                    foreach (Microsoft.AspNetCore.Mvc.ModelBinding.ModelError propertyError in error.Value.Errors)
                    {
                        errors.Add(propertyError.ErrorMessage);
                    }
                }
            }

            TempData["Redirect"] = RedirectPurpose.ResetPasswordModal;
            TempData["Model"] = JsonConvert.SerializeObject(model); // Pass the model to the view.
            TempData["Errors"] = errors.ToArray();
            return RedirectToAction("Index");
        }

        private string GetClientUserAgent() => Request.Headers["User-Agent"].ToString();
        private string GetRemoteClientIPv4() => HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

    }
}