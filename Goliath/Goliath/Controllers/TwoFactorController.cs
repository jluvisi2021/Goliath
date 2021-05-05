using Goliath.Attributes;
using Goliath.Enums;
using Goliath.Models;
using Goliath.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        private readonly IAccountRepository _repository;
        private readonly IUnauthorizedTimeoutsRepository _timeoutsRepository;
        private readonly ITwoFactorAuthorizeTokenRepository _authorizeTokenRepository;

        public TwoFactorController(IAccountRepository repository, IUnauthorizedTimeoutsRepository timeoutsRepository, ITwoFactorAuthorizeTokenRepository authorizeTokenRepository, ILogger<TwoFactorController> logger)
        {
            _repository = repository;
            _timeoutsRepository = timeoutsRepository;
            _authorizeTokenRepository = authorizeTokenRepository;
            _logger = logger;
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
                _logger.LogInformation($"Failure in attempting to redirect to Authenticate method. METHOD VALUES: [userAction = {userAction}, requireCode = {requireCode}]");
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
                twoFactorValid = await _repository.TwoFactorCodeValidAsync(user, model.TwoFactorCode);
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
                    _logger.LogInformation($"User {user.Id} ({user.UserName}) - Enabled two-factor authentication.");
                    return RedirectToAction(nameof(Index));

                case TwoFactorAction.DisableTwoFactor:
                    if (!passwordValid || !twoFactorValid)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Credentials.");
                        return View(model);
                    }

                    await _repository.SetTwoFactorDisabledAsync(user);

                    TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorDisabled;
                    _logger.LogInformation($"User {user.Id} ({user.UserName}) - Disabled two-factor authentication.");
                    return RedirectToAction(nameof(Index));

                case TwoFactorAction.GetVerificationCodes:
                    if (!passwordValid || !twoFactorValid)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Credentials.");
                        return View(model);
                    }

                    TempData[TempDataKeys.Redirect] = RedirectPurpose.VerificationCodes;
                    TempData[TempDataKeys.TwoFactorRecoveryCodes] = JsonConvert.SerializeObject(await _repository.GenerateUserRecoveryCodesAsync(user));
                    _logger.LogInformation($"User {user.Id} ({user.UserName}) - Requested two-factor verification codes.");
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

        [Route("get-verification-codes")]
        public IActionResult TwoFactorCodes()
        {
            return View();
        }

        // How this method works:
        /// <summary>
        /// <para> Sends an Sms two-factor code to a user and then redirects to a specified action. </para>
        /// <para>
        /// This method takes a Base64 serialized <see cref="ResendTwoFactorSmsCodeModel" /> model
        /// <paramref name="m" /> and attempts to decode it and then validate its attributes.
        /// </para>
        /// <para>
        /// This method REQUIRES a <b> "Action" </b> and a <b> "Controller" </b> to be passed
        /// through the model. This "Action" and "Controller" should reference a real action method
        /// path that this <see cref="IActionResult" /> returns to regardless of the outcome.
        /// </para>
        /// <para>
        /// The <see cref="ResendTwoFactorSmsCodeModel" /> can also be equipped with a "ReturnPath"
        /// which is optional but can request that if the process has completed successfully, send
        /// the user to this specific LocalUrl with Query arguments. In this case the user will not
        /// be sent to the "Action" and "Controller" but will be directly redirected to the ReturnPath.
        /// </para>
        /// <para>
        /// If the user is already signed in then we compare the username that is being requested to
        /// the current user which is signed in. <br /> Otherwise we check if the user has a valid
        /// Authorize cookie.
        /// </para>
        /// </summary>
        /// <param name="m"> A serialized model. </param>
        /// <returns> </returns>
        [IgnoreGoliathAuthorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("request-sms")]
        public async Task<IActionResult> SendSmsCode(string m)
        {
            ResendTwoFactorSmsCodeModel model;
            // Deserialize the resend two factor sms model.

            #region Parse & deserialize the parameter.

            try
            {
                model = JsonConvert.DeserializeObject<ResendTwoFactorSmsCodeModel>(Encoding.UTF8.GetString(Convert.FromBase64String(m)));
            }
            catch (Exception)
            {
                _logger.LogInformation($"[CHECK-1] Received invalid serialization for SendSmsCode(m). Model Data = [{m}]");
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailure;
                return RedirectToActionPermanent(nameof(AuthController.Index), GoliathControllers.AuthController);
            }

            #endregion Parse & deserialize the parameter.

            #region Validate object data (Non-Null + Valid Two-Factor method)

            // Check if the model exists and the username is not null.
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Controller) || string.IsNullOrWhiteSpace(model.Action))
            {
                _logger.LogInformation($"[CHECK-2] Received invalid serialization for SendSmsCode(m). Model Data = [{m}]");
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailure;
                TempData[TempDataKeys.HtmlMessage] = "Internal Error. Try again later.";
                return RedirectToActionPermanent(nameof(AuthController.Index), GoliathControllers.AuthController);
            }
            // Query for the user.
            ApplicationUser user = await _repository.GetUserByNameAsync(model.Username);

            // Check if the user exists and there two-factor method is SMS.
            if (user == null || user.TwoFactorMethod != (int)TwoFactorMethod.SmsVerify)
            {
                _logger.LogInformation($"[CHECK-3] Received invalid serialization for SendSmsCode(m). Model Data = [{m}]");
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailure;
                TempData[TempDataKeys.HtmlMessage] = "Internal Error. Try again later.";
                if (model.IsUrnRedirect)
                {
                    return RedirectToActionPermanent(model.Action, model.Controller);
                }
                return RedirectToActionPermanent(model.Action, model.Controller, new { userName = model.Username });
            }

            #endregion Validate object data (Non-Null + Valid Two-Factor method)

            #region Ensure a unauthorized user can request token

            // Check for Invalid Two-Factor Authorize Cookie + User not logged in
            if (!await _authorizeTokenRepository.TokenValidAsync(user.Id) && !User.Identity.IsAuthenticated)
            {
                _logger.LogInformation($"[CHECK-4A] Unauthorized request for user {user.Id} ({user.UserName} was rejected (Invalid Authorization Token). Model Data = [{m}]");
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailure;
                TempData[TempDataKeys.HtmlMessage] = "Invalid account session.";
                if (model.IsUrnRedirect)
                {
                    return RedirectToAction(model.Action, model.Controller);
                }
                return RedirectToAction(model.Action, model.Controller, new { userName = model.Username });
            }

            #endregion Ensure a unauthorized user can request token

            #region Ensure that a authorized user can request a token.

            // If the user is logged in and they are requesting a username that is not theirs.
            if (User.Identity.IsAuthenticated && (User.Identity.Name != model.Username))
            {
                _logger.LogInformation($"[CHECK-4B] Authorized request for user {user.Id} ({user.UserName} was rejected (Invalid Session). Model Data = [{m}]");
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailure;
                TempData[TempDataKeys.HtmlMessage] = "Invalid account session.";
                if (model.IsUrnRedirect)
                {
                    return RedirectToAction(model.Action, model.Controller);
                }
                return RedirectToAction(model.Action, model.Controller, new { userName = model.Username });
            }

            #endregion Ensure that a authorized user can request a token.

            #region Check if the user is on timeout

            // If the user has waited the timeout and is allowed to request another token.
            if (!await _timeoutsRepository.CanRequestResendTwoFactorSmsAsync(user.Id))
            {
                _logger.LogInformation($"[CHECK-5] Unauthorized request for user {user.Id} ({user.UserName} was rejected (Bad Timeout). Model Data = [{m}]");
                TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendFailureTimeout;
                TempData[TempDataKeys.HtmlMessage] = "Please wait before requesting another code.";
                if (model.IsUrnRedirect)
                {
                    return RedirectToAction(model.Action, model.Controller);
                }
                return RedirectToAction(model.Action, model.Controller, new { userName = model.Username });
            }

            #endregion Check if the user is on timeout

            // Send the user their code.
            await _repository.SendTwoFactorCodeSms(user);
            // Register the fact that they requested a two-factor code.
            await _timeoutsRepository.UpdateRequestAsync(user.Id, UnauthorizedRequest.RequestTwoFactorResendSms);
            // Send the success information back.
            TempData[TempDataKeys.Redirect] = RedirectPurpose.TwoFactorSmsResendSuccess;
            // If the redirect is a url with query parameters.
            _logger.LogInformation($"USER {user.Id} ({user.UserName}) - Received two-factor resend successfully. Model Data = [{m}]");
            if (model.IsUrnRedirect)
            {
                return LocalRedirect(model.ReturnPath);
            }
            return RedirectToAction(model.Action, model.Controller, new { userName = model.Username });
        }
    }
}