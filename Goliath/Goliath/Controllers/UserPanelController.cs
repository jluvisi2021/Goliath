using Goliath.Attributes;
using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using Goliath.Repository;
using Goliath.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the views for the User Panel which repersents the data that can be manipulated by a
    /// registered user.
    /// </summary>
    public sealed class UserPanelController : Controller
    {
        /// <summary>
        /// For interfacing with the ApplicationUser.
        /// </summary>
        private readonly IAccountRepository _accountRepository;

        private readonly ISmsVerifyTokensRepository _requestTable;
        private readonly IGoliathCaptchaService _captcha;

        public UserPanelController(IAccountRepository accountRepository, IGoliathCaptchaService captcha, ISmsVerifyTokensRepository requestTable)
        {
            _accountRepository = accountRepository;
            _captcha = captcha;
            _requestTable = requestTable;
        }

        public IActionResult Index()
        {
            return View("Profile");
        }

        public IActionResult Profile()
        {
            return View();
        }

        [PreventDuplicateRequest]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileSettingsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!await _captcha.IsCaptchaValidAsync())
            {
                ModelState.AddModelError(_captcha.CaptchaValidationError().Key, _captcha.CaptchaValidationError().Value);
                return View();
            }

            ApplicationUser goliathUser = await _accountRepository.GetUserFromContextAsync(User);
            // track if there has been any updates.
            bool hasChanged = false;

            if (HasValueChanged(model.BackgroundColor, goliathUser.BackgroundColor))
            {
                hasChanged = true;
                goliathUser.BackgroundColor = model.BackgroundColor;
            }
            if (HasValueChanged(model.DarkThemeEnabled, goliathUser.DarkTheme))
            {
                hasChanged = true;
                goliathUser.DarkTheme = model.DarkThemeEnabled;
            }
            if (HasValueChanged(model.NewEmail, goliathUser.Email))
            {
                if (await _accountRepository.DoesEmailExistAsync(model.NewEmail))
                {
                    ModelState.AddModelError(string.Empty, $"The email {model.NewEmail} is already in use.");
                    return View(model);
                }
                hasChanged = true;
                goliathUser.UnverifiedNewEmail = model.NewEmail;
                // Send a new verification email.
                await _accountRepository.GenerateNewEmailConfirmationTokenAsync(goliathUser, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
            }
            if (HasValueChanged(model.NewPhoneNumber, goliathUser.PhoneNumber))
            {
                if (await _accountRepository.DoesPhoneNumberExistAsync(model.NewPhoneNumber))
                {
                    ModelState.AddModelError(string.Empty, $"The phone number (+1) {model.NewPhoneNumber[0..3]}-{model.NewPhoneNumber[3..6]}-{model.NewPhoneNumber[6..]} is already in use.");
                    return View(model);
                }
                hasChanged = true;
                goliathUser.UnverifiedNewPhone = model.NewPhoneNumber;

                await _accountRepository.GenerateNewPhoneConfirmationTokenAsync(goliathUser, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
            }

            if (model.LogoutThreshold != null)
            {
                if (int.TryParse(model.LogoutThreshold, out int num))
                {
                    hasChanged = true;
                    goliathUser.LogoutThreshold = num;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Logout threshold must be a number.");
                    _captcha.DeleteCaptchaCookie();
                    return View(model);
                }
            }

            // If the new password entered does not match the users current password.

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (await _accountRepository.IsPasswordValidAsync(goliathUser, model.NewPassword))
                {
                    ModelState.AddModelError(string.Empty, "Your new password matches your old password.");
                    _captcha.DeleteCaptchaCookie();
                    return View(model);
                }
                Microsoft.AspNetCore.Identity.IdentityResult result = await _accountRepository.UpdatePasswordAsync(goliathUser, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    hasChanged = true;
                    goliathUser.LastPasswordUpdate = DateTime.UtcNow.ToString();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Your entry in \"Current Password\" field does not match your account password.");
                    _captcha.DeleteCaptchaCookie();
                    return View(model);
                }
            }

            if (hasChanged)
            {
                await _accountRepository.UpdateUserAsync(goliathUser);
                await _captcha.CacheNewCaptchaValidateAsync();
            }

            ModelState.Clear();
            TempData["ValuesUpdated"] = RedirectPurpose.SettingsUpdatedSuccess;
            return View();
        }

        private static bool HasValueChanged(string model, string user)
        {
            if (string.IsNullOrWhiteSpace(model))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(user) || user == "NULL")
            {
                return true;
            }
            if (!model.Equals(user))
            {
                return true;
            }
            return false;
        }

        [Route("userpanel/verify-phone")]
        public IActionResult ConfirmPhoneNumber()
        {
            return View();
        }

        [Route("userpanel/verify-phone")]
        [PreventDuplicateRequest]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPhoneNumber(VerifyPhoneNumberModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (!int.TryParse(model.Token, out _))
            {
                ModelState.AddModelError(string.Empty, "Token must be a number.");
                return View();
            }

            if (!await _captcha.IsCaptchaValidAsync())
            {
                ModelState.AddModelError(_captcha.CaptchaValidationError().Key, _captcha.CaptchaValidationError().Value);
                return View();
            }

            ApplicationUser user = await _accountRepository.GetUserFromContextAsync(User);

            if (!await _accountRepository.IsPasswordValidAsync(user, model.Password))
            {
                _captcha.DeleteCaptchaCookie();
                ModelState.AddModelError(string.Empty, "Incorrect Password.");
                return View();
            }

            if ((await _accountRepository.ConfirmPhoneAsync(user, model.Token)).Succeeded)
            {
                model.IsCompleted = true;
                user.UnverifiedNewPhone = string.Empty;
                await _captcha.CacheNewCaptchaValidateAsync();
                await _accountRepository.UpdateUserAsync(user);
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid or Expired Token.");
            return View(model);
        }

        /// <summary>
        /// Manages the default view for resending sms verify tokens.
        /// </summary>
        /// <returns> </returns>
        [Route("userpanel/resend-code")]
        public IActionResult ResendSmsVerifyToken()
        {
            return View();
        }

        /// <summary>
        /// Checks if the user is allowed to request another SMS verify token. The default time span
        /// until a token request is allowed is 2 hours.
        /// </summary>
        /// <param name="model"> </param>
        /// <returns> </returns>
        [HttpGet]
        [Route("userpanel/resend-code")]
        public async Task<IActionResult> ResendSmsVerifyToken(ResendSmsVerifyTokenModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IsSuccess = false;
                return View();
            }
            ApplicationUser user = await _accountRepository.GetUserByNameAsync(model.Username);
            // Make sure the user exists and is the current user logged in.
            if (user == null || !user.UserName.Equals(User.Identity.Name))
            {
                model.IsSuccess = false;
                return View();
            }
            if (await _requestTable.IsUserResendValidAsync(user.Id))
            {
                // User can request a resend.
                model.IsSuccess = true;
                await _requestTable.AddRequestAsync(user.Id);
                await _accountRepository.GenerateNewPhoneConfirmationTokenAsync(user);
                return View(model);
            }
            model.IsSuccess = false;
            return View();
        }

        /// <summary>
        /// Accept a parameter in the URL and pass it into the model.
        /// </summary>
        /// <param name="username"> </param>
        /// <returns> </returns>
        [HttpPost]
        [Route("userpanel/resend-code")]
        public IActionResult ResendSmsVerifyToken(string username)
        {
            return View(new ResendSmsVerifyTokenModel()
            {
                Username = username
            });
        }

        /// <summary>
        /// Returns a specific partial view in Ajax. <br /> Specifically useful for loading a part
        /// of the screen that is unknown at runtime.
        /// </summary>
        /// <param name="partialName"> File name of partial view. </param>
        /// <returns> </returns>
        public ActionResult GetModule(string partialName) => PartialView($"~/Views/UserPanel/{partialName}.cshtml");

        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.SignOutAsync();

            // Redirect the user to the main login screen with information that the user has just
            // been logged out.
            TempData["Redirect"] = RedirectPurpose.LogoutSuccess;
            return RedirectToAction("Index", "Auth");
        }

        public IActionResult Database() => View();

        public IActionResult Utilities() => View();

        [ResponseCache(Duration = 14400, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Help() => View();

        [ResponseCache(Duration = 14400, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult About() => View();

        [ResponseCache(Duration = 14400, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult BuildInfo() => View();

        public IActionResult AdminPanel() => View();

        private string GetClientUserAgent() => Request.Headers["User-Agent"].ToString();

        private string GetRemoteClientIPv4() => HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}