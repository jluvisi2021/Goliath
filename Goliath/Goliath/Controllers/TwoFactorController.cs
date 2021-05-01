using Goliath.Attributes;
using Goliath.Enums;
using Goliath.Models;
using Goliath.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the routing/validation for two factor authentication. All two-factor authentication
    /// views are stored in userpanel.
    /// </summary>
    [Route("userpanel/2fa")]
    [GoliathAuthorize("Profile")]
    public class TwoFactorController : Controller
    {
        private readonly IAccountRepository _repository;
        private readonly IUnauthorizedTimeoutsRepository _timeoutsRepository;
        private readonly ITwoFactorAuthorizeTokenRepository _authorizeTokenRepository;

        public TwoFactorController(IAccountRepository repository, IUnauthorizedTimeoutsRepository timeoutsRepository, ITwoFactorAuthorizeTokenRepository authorizeTokenRepository)
        {
            _repository = repository;
            _timeoutsRepository = timeoutsRepository;
            _authorizeTokenRepository = authorizeTokenRepository;
        }

        /// <summary>
        /// The index view which displays the options for two factor.
        /// </summary>
        /// <returns> </returns>
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// View for setting up two factor authentication for a SMS verification.
        /// </summary>
        /// <returns> </returns>
        [Route("sms")]
        public IActionResult SetupSms2FA()
        {
            return View();
        }

        [HttpGet]
        [Route("authenticate-changes")]
        public IActionResult Authenticate(TwoFactorAction userAction, string requireCode)
        {
            if (userAction == TwoFactorAction.Error || !bool.TryParse(requireCode, out _))
            {
                TempData[TempDataKeys.HtmlMessage] = HttpUtility.HtmlEncode("We encountered a problem processing this request.");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(new TwoFactorAuthenticateRedirectModel()
                {
                    Action = userAction,
                    TwoFactorCodeRequired = bool.Parse(requireCode)
                });
            }
        }

        [Route("authenticate-changes")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        [HttpPost]
        public async Task<IActionResult> Authenticate(TwoFactorAuthenticateRedirectModel model)
        {
            // If the model has not been submitted yet.
            if (!model.IsSubmitted)
            {
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await _repository.GetUserFromContextAsync(User);

            #region Ensure Two-Factor enabled; Verify token

            if (model.Action != TwoFactorAction.EnableTwoFactor && !user.TwoFactorEnabled)
            {
                ModelState.AddModelError(string.Empty, "You must have two factor enabled to do this.");
                return View(model);
            }

            bool passwordValid = await _repository.IsPasswordValidAsync(user, model.Password);
            bool twoFactorValid = false;
            if (model.TwoFactorCodeRequired)
            {
                twoFactorValid = model.TwoFactorCode == "12345"; // Testing Value
            }

            #endregion Ensure Two-Factor enabled; Verify token

            // Perform actions accordingly
            switch (model.Action)
            {
                case TwoFactorAction.EnableTwoFactor:
                    if (!passwordValid)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Password.");
                        return View(model);
                    }
                    await _repository.SetTwoFactorEnabledAsync(user, TwoFactorMethod.SmsVerify);

                    TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorEnabled;
                    TempData[TempDataKeys.TwoFactorRecoveryCodes] = JsonConvert.SerializeObject(await _repository.GenerateUserRecoveryCodesAsync(user));
                    return RedirectToAction(nameof(Index));

                case TwoFactorAction.DisableTwoFactor:
                    if (!passwordValid || !twoFactorValid)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Credentials.");
                        return View(model);
                    }

                    await _repository.SetTwoFactorDisabledAsync(user);

                    TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorDisabled;
                    return RedirectToAction(nameof(Index));

                case TwoFactorAction.GetVerificationCodes:
                    if (!passwordValid || !twoFactorValid)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Credentials.");
                        return View(model);
                    }

                    TempData[TempDataKeys.Redirect] = RedirectPurpose.VerificationCodes;
                    TempData[TempDataKeys.TwoFactorRecoveryCodes] = JsonConvert.SerializeObject(await _repository.GenerateUserRecoveryCodesAsync(user));
                    return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// <summary>
        /// View for setting up 2FA through a mobile app like Microsoft Authenticate.
        /// </summary>
        /// <returns> </returns>
        [Route("app")]
        public IActionResult SetupApp2FA()
        {
            return View();
        }

        /// <summary>
        /// Sends an sms code to the user and then redirects to a specific controller and action.
        /// Pass in a serialized model encoded in Base64.
        /// </summary>
        /// <param name="m"> </param>
        /// <returns> </returns>
        [IgnoreGoliathAuthorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("request-sms")]
        public async Task<IActionResult> SendSmsCode(string m)
        {
            ResendTwoFactorSmsCodeModel model;
            // Deserialize the resend two factor sms model.
            try
            {
                model = JsonConvert.DeserializeObject<ResendTwoFactorSmsCodeModel>(Encoding.UTF8.GetString(Convert.FromBase64String(m)));
            }
            catch (Exception)
            {
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailure;
                return RedirectToActionPermanent(nameof(AuthController.Index), GoliathControllers.AuthController);
            }

            #region Validate object data

            if (model == null || string.IsNullOrWhiteSpace(model.Username))
            {
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailure;
                return RedirectToActionPermanent(nameof(AuthController.Index), GoliathControllers.AuthController);
            }
            ApplicationUser user = await _repository.GetUserByNameAsync(model.Username);
            if (user == null)
            {
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailure;
                return RedirectToAction(model.Action, model.Controller, new { userName = model.Username });
            }

            #endregion Validate object data

            #region Ensure user can request token

            // Invalid Two-Factor Authorize Cookie
            if (!await _authorizeTokenRepository.TokenValidAsync(user.Id))
            {
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailure;
                return RedirectToAction(model.Action, model.Controller, new { userName = model.Username });
            }
            if (!await _timeoutsRepository.CanRequestResendTwoFactorSmsAsync(user.Id))
            {
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailureTimeout;
                return RedirectToAction(model.Action, model.Controller, new { userName = model.Username });
            }

            #endregion Ensure user can request token

            await _repository.SendTwoFactorCodeSms(user);
            await _timeoutsRepository.UpdateRequestAsync(user.Id, UnauthorizedRequest.RequestTwoFactorResendSms);
            TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendSuccess;
            return RedirectToAction(model.Action, model.Controller, new { userName = model.Username });
        }
    }
}