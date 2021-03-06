﻿using Goliath.Attributes;
using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using Goliath.Models.Accounts;
using Goliath.Repository;
using Goliath.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the views for the User Panel which repersents the data that can be manipulated by a
    /// registered user.
    /// </summary>
    public sealed class UserPanelController : Controller
    {
        private readonly ILogger _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly ISmsVerifyTokensRepository _requestTable;
        private readonly IUserTimeoutsRepository _timeouts;
        private readonly IGoliathCaptchaService _captcha;
        private readonly ICookieManager _cookies;

        public UserPanelController(IAccountRepository accountRepository, IGoliathCaptchaService captcha, ISmsVerifyTokensRepository requestTable, ICookieManager cookies, IUserTimeoutsRepository timeouts, ILogger<AuthController> logger)
        {
            _accountRepository = accountRepository;
            _captcha = captcha;
            _requestTable = requestTable;
            _cookies = cookies;
            _logger = logger;
            _timeouts = timeouts;
        }

        [HttpPost]
        [Route("userpanel/authorize")]
        public IActionResult NotAuthenticated(string title)
        {
            return View(new NotAuthorizedModel()
            {
                Title = title
            });
        }

        [Route("userpanel/authorize")]
        [HttpGet]
        public IActionResult NotAuthenticated(NotAuthorizedModel model)
        {
            return View(model);
        }

        [GoliathAuthorize(nameof(Profile))]
        [Route("sms-code-sent")]
        public IActionResult SentSmsCode()
        {
            return View();
        }

        [GoliathAuthorize(nameof(Profile))]
        public IActionResult Index()
        {
            return View(nameof(Profile));
        }

        [GoliathAuthorize(nameof(Profile))]
        public IActionResult Profile()
        {
            return View();
        }

        [GoliathAuthorize(nameof(Profile))]
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

            if (!await _timeouts.CanRequestProfileSettingsUpdateAsync(goliathUser.Id))
            {
                ModelState.AddModelError(string.Empty, "Please wait before updating your profile again.");
                return View();
            }

           #region Simple value updates

            // ** Values which do not need database checking
            goliathUser.BackgroundColor = model.BackgroundColor;
            goliathUser.DarkTheme = model.DarkThemeEnabled;
            goliathUser.LogoutThreshold = int.Parse(model.LogoutThreshold);
            // ** If settings which require a two-factor code are updated.
            bool requiresTwoFactor = false;

            #endregion Simple value updates

            #region Validating email

            if (!string.IsNullOrWhiteSpace(model.NewEmail))
            {
                if (await _accountRepository.DoesEmailExistAsync(model.NewEmail))
                {
                    ModelState.AddModelError(string.Empty, $"The email {model.NewEmail} is currently in use.");
                    return View();
                }
                else
                {
                    goliathUser.UnverifiedNewEmail = model.NewEmail;
                    requiresTwoFactor = true;
                }
            }

            #endregion Validating email

            #region Validating phone number

            if (!string.IsNullOrWhiteSpace(model.NewPhoneNumber))
            {
                if (await _accountRepository.DoesPhoneNumberExistAsync(model.NewPhoneNumber))
                {
                    ModelState.AddModelError(string.Empty, $"The phone number {model.NewPhoneNumber} is currently in use.");
                    return View();
                }
                else
                {
                    goliathUser.UnverifiedNewPhone = model.NewPhoneNumber;
                    requiresTwoFactor = true;
                }
            }

            #endregion Validating phone number

            #region Validating password

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (await _accountRepository.IsPasswordValidAsync(goliathUser, model.NewPassword))
                {
                    ModelState.AddModelError(string.Empty, "Your new password must be different then your previous password.");
                    return View();
                }
                IdentityResult result = await _accountRepository.UpdatePasswordAsync(goliathUser, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    _captcha.DeleteCaptchaCookie();
                    ModelState.AddModelError(string.Empty, "Your entry in \"Current Password\" does not match your current password.");
                    return View();
                }
                goliathUser.LastPasswordUpdate = DateTime.UtcNow.ToString();
                requiresTwoFactor = true;
            }

            #endregion Validating password

            #region Check two-factor codes

            if (goliathUser.TwoFactorEnabled && requiresTwoFactor)
            {
                if (string.IsNullOrWhiteSpace(model.TwoFactorCode))
                {
                    _captcha.DeleteCaptchaCookie();
                    ModelState.AddModelError(string.Empty, "Your two-factor code is invalid.");
                    return View();
                }
                if (!await _accountRepository.TwoFactorCodeValidAsync(goliathUser, model.TwoFactorCode))
                {
                    _captcha.DeleteCaptchaCookie();
                    ModelState.AddModelError(string.Empty, "Your two-factor code is invalid.");
                    return View();
                }
            }

            #endregion Check two-factor codes

            #region Sending potential verification emails

            // If all of the user settings are correct then check if we need to send messages to
            // update phone/email.
            if (!string.IsNullOrWhiteSpace(model.NewEmail))
            {
                await _accountRepository.GenerateNewEmailConfirmationTokenAsync(goliathUser, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
            }
            if (!string.IsNullOrWhiteSpace(model.NewPhoneNumber))
            {
                await _accountRepository.GenerateNewPhoneConfirmationTokenAsync(goliathUser, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()));
            }

            #endregion Sending potential verification emails

            #region Updating/Caching
            // Update all of the changed values.
            await _accountRepository.UpdateUserAsync(goliathUser);
            await _captcha.CacheNewCaptchaValidateAsync();
            await _timeouts.UpdateRequestAsync(goliathUser.Id, UserRequest.UpdateProfileSettings);
            #endregion Updating/Caching

            ModelState.Clear();
            _logger.LogInformation($"{goliathUser.Id} ({goliathUser.UserName}) - Updated profile settings successfully.");
            TempData[TempDataKeys.Redirect] = RedirectPurpose.SettingsUpdatedSuccess;
            return View();
        }

        [GoliathAuthorize(nameof(Profile))]
        [Route("userpanel/verify-phone")]
        public IActionResult ConfirmPhoneNumber()
        {
            return View();
        }

        [GoliathAuthorize(nameof(Profile))]
        [Route("userpanel/verify-phone")]
        [PreventDuplicateRequest]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPhoneNumber(VerifyPhoneNumberModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                return View();
            }

            #region Validate token data type & captcha

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

            #endregion Validate token data type & captcha

            ApplicationUser user = await _accountRepository.GetUserFromContextAsync(User);

            #region Validating password & ensuring token validity

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
                _logger.LogInformation($"{user.Id} ({user.UserName}) - Confirmed phone number successfully.");
                return View(model);
            }

            #endregion Validating password & ensuring token validity

            ModelState.AddModelError(string.Empty, "Invalid or Expired Token.");
            return View(model);
        }

        /// <summary>
        /// Manages the default view for resending sms verify tokens.
        /// </summary>
        /// <returns> </returns>
        [GoliathAuthorize(nameof(Profile))]
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
        [GoliathAuthorize(nameof(Profile))]
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
                _logger.LogInformation($"{user.Id} ({user.UserName}) - Requested an SMS verification resend successfully.");
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
        [GoliathAuthorize(nameof(Profile))]
        [Route("userpanel/resend-code")]
        public IActionResult ResendSmsVerifyToken(string username)
        {
            return View(new ResendSmsVerifyTokenModel()
            {
                Username = username
            });
        }

        [GoliathAuthorize(nameof(Profile))]
        [Route("clear-traceback")]
        public async Task<IActionResult> ClearLoginTraceback()
        {
            TempData[TempDataKeys.HtmlMessage] = "Cleared all login tracebacks.";
            await _accountRepository.ClearLoginTracebackAsync(await _accountRepository.GetUserFromContextAsync(User));
            return RedirectToAction(nameof(Profile));
        }

        /// <summary>
        /// Returns a specific partial view in Ajax. <br /> Specifically useful for loading a part
        /// of the screen that is unknown at runtime.
        /// </summary>
        /// <param name="partialName"> File name of partial view. </param>
        /// <returns> </returns>
        public ActionResult GetModule(string partialName) => PartialView($"~/Views/UserPanel/{partialName}.cshtml");

        [GoliathAuthorize(nameof(Profile))]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.SignOutAsync();
            if (User != null)
            {
                _logger.LogInformation($"({User.Identity.Name}) - Logged out.");
            }
            if (_cookies.HasCookie(CookieKeys.TwoFactorAuthorizeCookie))
            {
                _cookies.DeleteCookie(CookieKeys.TwoFactorAuthorizeCookie);
            }

            // Redirect the user to the main login screen with information that the user has just
            // been logged out.
            TempData[TempDataKeys.Redirect] = RedirectPurpose.LogoutSuccess;
            return RedirectToAction(nameof(AuthController.Index), GoliathControllers.AuthController);
        }

        [GoliathAuthorize(nameof(Profile))]
        [Route("userpanel/download-data")]
        public async Task<IActionResult> DownloadData()
        {
            ApplicationUser user = await _accountRepository.GetUserFromContextAsync(User);
            if (!await _timeouts.CanRequestDataDownloadAsync(user.Id))
            {
                TempData[TempDataKeys.HtmlMessage] = "Please wait before downloading your data again.";
                return RedirectToAction(nameof(Profile));
            }

            await Task.Delay(5000); // Wait 5 seconds.
            await _timeouts.UpdateRequestAsync(user.Id, UserRequest.RequestDataDownload);
            string data = await _accountRepository.UserToJsonAsync(user);
            return File(Encoding.ASCII.GetBytes(data), "text/plain", user.UserName + "-data.json");
        }

        [GoliathAuthorize(nameof(Profile))]
        [Route("userpanel/download-data-enc")]
        public async Task<IActionResult> DownloadDataEncrypted()
        {
            ApplicationUser user = await _accountRepository.GetUserFromContextAsync(User);
            if (!await _timeouts.CanRequestDataDownloadAsync(user.Id))
            {
                TempData[TempDataKeys.HtmlMessage] = "Please wait before downloading your data again.";
                return RedirectToAction(nameof(Profile));
            }

            await Task.Delay(5000); // Wait 5 seconds.
            await _timeouts.UpdateRequestAsync(user.Id, UserRequest.RequestDataDownload);

            string key = GoliathHelper.GenerateSecureRandomString();
            string data = AesHelper.EncryptText(await _accountRepository.UserToJsonAsync(user), key, user.UserName);

            await _accountRepository.GenerateNewDataEncryptionEmailAsync(user, new DeviceParser(GetClientUserAgent(), GetRemoteClientIPv4()), key, Url.ActionLink(nameof(GoliathAesController.DecryptUserData), GoliathControllers.AesController));
            return File(Encoding.ASCII.GetBytes(data), "text/plain", user.UserName + "-data.txt");
        }

        [GoliathAuthorize(nameof(Profile))]
        [Route("userpanel/delete")]
        public IActionResult DeleteAccount()
        {
            return View();
        }

        [GoliathAuthorize(nameof(Profile))]
        [Route("userpanel/delete")]
        [HttpPost, ValidateAntiForgeryToken, PreventDuplicateRequest]
        public async Task<IActionResult> DeleteAccount(DeleteAccountModel model)
        {
            

            if(!ModelState.IsValid)
            {
                TempData[TempDataKeys.HtmlMessage] = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).AsEnumerable().Reverse().Aggregate((a, b) => a + "<br />" + b);
                return RedirectToAction(nameof(DeleteAccount));
            }
            if (!model.Username.Equals(User.Identity.Name))
            {
                ModelState.AddModelError(string.Empty, "Invalid Username.");
            }
            var user = await _accountRepository.GetUserFromContextAsync(User);

            if (!await _accountRepository.IsPasswordValidAsync(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid Password.");
            }
            if(user.TwoFactorEnabled)
            {
                if (!await _accountRepository.TwoFactorCodeValidAsync(user, model.TwoFactorCode))
                {
                    ModelState.AddModelError(string.Empty, "Invalid Two-Factor Code.");
                }
            }
            // Check model state again.
            if (!ModelState.IsValid)
            {
                TempData[TempDataKeys.HtmlMessage] = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).AsEnumerable().Aggregate((a, b) => a + "<br />" + b);
                return RedirectToAction(nameof(DeleteAccount));
            }
            await _accountRepository.SignOutAsync();
            await _accountRepository.DeleteUserAccountAsync(user);
            TempData[TempDataKeys.Redirect] = RedirectPurpose.AccountDeleted;
            return RedirectToAction(nameof(AuthController.Login), GoliathControllers.AuthController);
        }

        [GoliathAuthorize(nameof(Database))]
        public IActionResult Database() => View();

        [GoliathAuthorize(nameof(Utilities))]
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