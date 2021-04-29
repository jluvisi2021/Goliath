using Goliath.Attributes;
using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using Goliath.Repository;
using Goliath.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the Views for Authentication. <br /> ButtonID indicates the radio button to be checked.
    /// </summary>
    [Route("account")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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

        private readonly IGoliathCaptchaService _captcha;

        private readonly IUnauthorizedTimeoutsRepository _timeoutsRepository;

        public AuthController
            (IAccountRepository accountRepository,
            SignInManager<ApplicationUser> signInManager,
            IGoliathCaptchaService captcha,
            IUnauthorizedTimeoutsRepository timeoutsRepository)
        {
            _accountRepository = accountRepository;
            _signInManager = signInManager;
            _captcha = captcha;
            _timeoutsRepository = timeoutsRepository;
        }

        public IActionResult Index()
        {
            // If the user is signed in redirect them to the user panel.
            if (_signInManager.IsSignedIn(User))
            {
                // Fixes an error where the website would throw exception if the database was
                // refreshed but the auth cookie was still valid.
                if (User == null)
                {
                    _accountRepository.SignOutAsync();
                }
                else
                {
                    return RedirectToAction("Index", "UserPanel");
                }
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
        [PreventDuplicateRequest]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(SignInModel signInModel)
        {
            ViewData["ButtonID"] = ButtonID.Login;

            // Check if fields are entered and match checks.
            if (!ModelState.IsValid)
            {
                return View(signInModel);
            }

            // Model State is Valid; Check Captcha
            if (!await _captcha.IsCaptchaValidAsync())
            {
                ModelState.AddModelError(_captcha.CaptchaValidationError().Key, _captcha.CaptchaValidationError().Value);
                return View(signInModel);
            }

            // Attempt to sign the user in.
            SignInResult result = await _accountRepository.PasswordSignInAsync(signInModel);

            if (result.Succeeded)
            {
                // Store the fact that the CAPTCHA was completed successfully.
                await _captcha.CacheNewCaptchaValidateAsync();
                // Change the time of last login.
                await _accountRepository.UpdateLastLoginAsync(signInModel.Username);
                // Redirect
                return RedirectToAction("Index", "UserPanel");
            }
            // Result failed. Check for reason why.

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Please try again later.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "You must verify your email!");
            }
            else if (result.RequiresTwoFactor)
            {
                //TODO: Store a hash in tempdata of the user's
                return RedirectToAction(nameof(TwoFactorValidation), new { userName = signInModel.Username });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Credentials.");
            }

            // Invalidate Captcha Cookie.
            _captcha.DeleteCaptchaCookie();
            // Return view with errors.
            return View(signInModel);
        }


        [Route("validate")]
        [HttpGet]
        public async Task<IActionResult> TwoFactorValidation(string userName)
        {
            ViewData["ButtonID"] = ButtonID.Login;
            if(string.IsNullOrWhiteSpace(userName))
            {
                TempData["Redirect"] = RedirectPurpose.TwoFactorSmsResendFailure;
                return RedirectToActionPermanent(nameof(Login));
            }
            var user = await _accountRepository.GetUserByNameAsync(userName);
            if(user == null)
            {
                TempData["Redirect"] = RedirectPurpose.TwoFactorSmsResendFailure;
                return RedirectToActionPermanent(nameof(Login));
            }
            if(user.TwoFactorMethod == (int)TwoFactorMethod.SmsVerify && await _timeoutsRepository.CanRequestTwoFactorSmsAsync(user.Id))
            {

                    await _accountRepository.SendTwoFactorCodeSms(user);
                    await _timeoutsRepository.UpdateRequestAsync(user.Id, UnauthorizedRequest.InitalTwoFactorRequestSms);
                
            }
    
            return View(nameof(TwoFactorValidation), new TwoFactorAuthenticateModel()
            {
                InputUsername = userName,
                UserMethod = user.TwoFactorMethod == (int)TwoFactorMethod.SmsVerify ? TwoFactorMethod.SmsVerify : TwoFactorMethod.MobileAppVerify,
            });
        }

        [Route("validate")]
        [PreventDuplicateRequest]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TwoFactorValidation(TwoFactorAuthenticateModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _accountRepository.GetUserByNameAsync(model.InputUsername);
            if(model.UserMethod == TwoFactorMethod.SmsVerify)
            {
                if(await _accountRepository.TwoFactorCodeValidAsync(user, model.InputTwoFactorCode))
                {
                    var result = await _accountRepository.AuthorizeUserTwoFactorAsync(user, model.InputTwoFactorCode, model.RememberMe);

                    if (result.Succeeded)
                    {
                        // Store the fact that the CAPTCHA was completed successfully.
                        await _captcha.CacheNewCaptchaValidateAsync();
                        // Change the time of last login.
                        await _accountRepository.UpdateLastLoginAsync(user);
                        // Redirect
                        return RedirectToAction("Index", "UserPanel");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Bad Login. Try again later.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Two-Factor Code.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Is an app.");
                return View(model);
                // Is an app.
            }
        }

        [Route("register/goliath")]
        public IActionResult Register()
        {
            ViewData["ButtonID"] = ButtonID.Register;
            return View();
        }

        [Route("register/goliath")]
        [PreventDuplicateRequest]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(SignUpUserModel model)
        {
            ViewData["ButtonID"] = ButtonID.Register;

            // Check if fields match checks.
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Model State Valid; Check Captcha
            if (!await _captcha.IsCaptchaValidAsync())
            {
                ModelState.AddModelError(_captcha.CaptchaValidationError().Key, _captcha.CaptchaValidationError().Value);
                return View(model);
            }

            // Attempt to create a new user.
            IdentityResult result = await _accountRepository.CreateUserAsync(model,
                new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));

            if (result.Succeeded)
            {
                // Registration is Valid.
                ModelState.Clear();
                await _captcha.CacheNewCaptchaValidateAsync();
                // Send the user to the index with register tempdata.
                TempData["Redirect"] = RedirectPurpose.RegisterSuccess;
                // Send back to index.
                return RedirectToAction("Index");
            }
            // Registration Failed

            _captcha.DeleteCaptchaCookie();
            // For every error that is created during registration we add that to the eventual
            // bootstrap modal.
            foreach (IdentityError errorMessage in result.Errors)
            {
                ModelState.AddModelError(string.Empty, errorMessage.Description);
            }

            return View(model);
        }

        [Route("register/method")]
        public IActionResult RegisterMethod()
        {
            ViewData["ButtonID"] = ButtonID.Register;
            return View();
        }

        [Route("restore/password")]
        public IActionResult ForgotPassword()
        {
            ViewData["ButtonID"] = ButtonID.ForgotPassword;
            return View();
        }

        [Route("restore/password")]
        [PreventDuplicateRequest]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            ViewData["ButtonID"] = ButtonID.ForgotPassword;
            if (!ModelState.IsValid)
            {
                return View();
            }
            // Verify that the user exists with the specified email.
            ApplicationUser user = await _accountRepository.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"We could not find user: {model.Email}");
                return View();
            }
            // User Exists; Check Valid Captcha
            if (!await _captcha.IsCaptchaValidAsync())
            {
                ModelState.AddModelError(_captcha.CaptchaValidationError().Key, _captcha.CaptchaValidationError().Value);
                return View();
            }

            // User exists but username does not match email.
            if (!user.UserName.Equals(model.Username))
            {
                ModelState.AddModelError(string.Empty, $"Invalid Username \"{model.Username}\" for {model.Email}");
                return View();
            }
            // Email is not confirmed.
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Confirm your account before resetting your password.");
                return View();
            }

            // Generate a token as well as a user agent.
            await _accountRepository.GenerateForgotPasswordTokenAsync(user, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
            // Indicate to the View that the email was sent.
            model.IsEmailSent = true;
            // Clear all fields.
            ModelState.Clear();
            // Cache Captcha Validation.
            await _captcha.CacheNewCaptchaValidateAsync();
            await _timeoutsRepository.UpdateRequestAsync(user.Id, UnauthorizedRequest.RequestForgotPasswordEmail);
            return View(model);
        }

        [Route("restore/username")]
        public IActionResult ForgotUsername()
        {
            ViewData["ButtonID"] = ButtonID.ForgotUsername;
            return View();
        }

        [Route("restore/username")]
        [PreventDuplicateRequest]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotUsername(ForgotUsernameModel model)
        {
            ViewData["ButtonID"] = ButtonID.ForgotUsername;
            if (!ModelState.IsValid)
            {
                return View();
            }

            ApplicationUser user = await _accountRepository.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"We could not find user: {model.Email}");
                return View();
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Please confirm your account before doing this.");
                return View();
            }

            if (!await _captcha.IsCaptchaValidAsync())
            {
                ModelState.AddModelError(_captcha.CaptchaValidationError().Key, _captcha.CaptchaValidationError().Value);
                return View();
            }

            await _accountRepository.GenerateUsernameAsync(user, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
            model.IsEmailSent = true;
            ModelState.Clear();
            await _captcha.CacheNewCaptchaValidateAsync();
            await _timeoutsRepository.UpdateRequestAsync(user.Id, UnauthorizedRequest.RequestUsernameEmail);
            return View(model);
        }

        [Route("verify")]
        public IActionResult VerifyEmailAsync()
        {
            ViewData["ButtonID"] = ButtonID.VerifyEmail;
            return View();
        }

        [Route("verify")]
        [PreventDuplicateRequest]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmailAsync(EmailConfirmModel model)
        {
            ViewData["ButtonID"] = ButtonID.VerifyEmail;

            if (!ModelState.IsValid)
            {
                return View();
            }

            // Verify that the user exists with the specified email.
            ApplicationUser user = await _accountRepository.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"We could not find user: {model.Email}.");
                ModelState.AddModelError(string.Empty, "If you are verifying a new email then put your older one instead.");

                return View();
            }

            bool newEmail = !string.IsNullOrWhiteSpace(user.UnverifiedNewEmail);

            if (user.EmailConfirmed && !newEmail)
            {
                model.IsConfirmed = true;
                ModelState.AddModelError(string.Empty, "Account already verified.");
                return View(model);
            }

            if (!await _captcha.IsCaptchaValidAsync())
            {
                ModelState.AddModelError(_captcha.CaptchaValidationError().Key, _captcha.CaptchaValidationError().Value);
                return View();
            }

            if (newEmail)
            {
                await _accountRepository.GenerateNewEmailConfirmationTokenAsync(user, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
            }
            else
            {
                // Generate a token as well as a user agent.
                await _accountRepository.GenerateEmailConfirmationTokenAsync(user, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
            }

            // Indicate to the View that the email was sent.
            model.IsEmailSent = true;
            // Clear all fields.
            ModelState.Clear();
            await _captcha.CacheNewCaptchaValidateAsync();
            await _timeoutsRepository.UpdateRequestAsync(user.Id, UnauthorizedRequest.RequestVerificationEmail);
            return View(model);
        }

        /// <summary>
        /// Verifies an email given a UID and TOKEN. <br /> Does not contain a view and redirects to
        /// index view. <br /> The result of the method call is returned by the Bootstrap modal.
        /// </summary>
        /// <param name="uid"> </param>
        /// <param name="token"> </param>
        /// <returns> </returns>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmailAsync(string uid, string token)
        {
            // Check for blanks in URL query.
            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(token))
            {
                // Alert to the view that the verification failed.
                TempData["Redirect"] = RedirectPurpose.VerifiedEmailFailure;
                return RedirectToAction("Index");
            }

            token = token.Replace(' ', '+');

            // Check to make sure the token has not expired or is invalid.
            IdentityResult result = await _accountRepository.ConfirmEmailAsync(uid, token);
            if (result.Succeeded)
            {
                // If the email confirmation is a success then we can pass that info into the view.
                TempData["Redirect"] = RedirectPurpose.VerifiedEmailSuccess;
                return RedirectToAction("Index");
            }
            // Alert to the view that the verification failed.
            TempData["Redirect"] = RedirectPurpose.VerifiedEmailFailure;
            return RedirectToAction("Index");
        }

        [HttpGet("forgot-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            ViewData["ButtonID"] = ButtonID.ForgotPassword;

            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(token))
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

        [PreventDuplicateRequest]
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