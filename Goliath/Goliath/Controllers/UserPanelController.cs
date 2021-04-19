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

        private readonly IGoliathCaptchaService _captcha;

        public UserPanelController(IAccountRepository accountRepository, IGoliathCaptchaService captcha)
        {
            _accountRepository = accountRepository;
            _captcha = captcha;
        }

        public IActionResult Index()
        {
            return View("Profile");
        }

        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileSettingsGeneralModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await _captcha.IsCaptchaValidAsync())
            {
                ModelState.AddModelError(_captcha.CaptchaValidationError().Key, _captcha.CaptchaValidationError().Value);
                return View();
            }

            ApplicationUser goliathUser = await _accountRepository.GetUserFromContext(User);
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
                await _accountRepository.GenerateNewEmailConfirmationToken(goliathUser, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
            }
            if (HasValueChanged(model.NewPhoneNumber, goliathUser.PhoneNumber))
            {
                if (await _accountRepository.DoesPhoneNumberExistAsync(model.NewPhoneNumber))
                {
                    ModelState.AddModelError(string.Empty, $"The phone number {model.NewPhoneNumber} is already in use.");
                    return View(model);
                }
                hasChanged = true;
                goliathUser.UnverifiedNewPhone = model.NewPhoneNumber;

                await _accountRepository.GenerateNewPhoneConfirmationToken(goliathUser, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
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
                if (await _accountRepository.IsPasswordValid(goliathUser, model.NewPassword))
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
                await _accountRepository.UpdateUser(goliathUser);
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

            ApplicationUser user = await _accountRepository.GetUserFromContext(User);

            if (!await _accountRepository.IsPasswordValid(user, model.Password))
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
                await _accountRepository.UpdateUser(user);
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid or Expired Token.");
            return View(model);
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