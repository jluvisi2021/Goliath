﻿using Goliath.Attributes;
using Goliath.Enums;
using Goliath.Models;
using Goliath.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

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

        public TwoFactorController(IAccountRepository repository)
        {
            _repository = repository;
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
                return Content("We encountered an error. Please try again.");
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
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            
            ApplicationUser user = await _repository.GetUserFromContextAsync(User);

            if (model.Action != TwoFactorAction.EnableTwoFactor && !user.TwoFactorEnabled)
            {
                ModelState.AddModelError(string.Empty, "You must have two factor enabled to do this.");
                return View(model);
            }

            bool passwordValid = await _repository.IsPasswordValidAsync(user, model.Password);
            bool twoFactorValid = false;
            if (model.TwoFactorCodeRequired)
            {
                twoFactorValid = model.TwoFactorCode == "12345";
            }

            switch (model.Action)
            {
                case TwoFactorAction.EnableTwoFactor:
                    if (!passwordValid)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Password.");
                        return View(model);
                    }
                    await _repository.SetTwoFactorEnabledAsync(user, TwoFactorMethod.SmsVerify);

                    TempData["Redirect"] = RedirectPurpose.TwoFactorEnabled;
                    TempData["RecoveryCodes"] = JsonConvert.SerializeObject(await _repository.GenerateUserRecoveryCodesAsync(user));
                    return RedirectToAction("Index");

                case TwoFactorAction.DisableTwoFactor:
                    if (!passwordValid || !twoFactorValid)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Credentials.");
                        return View(model);
                    }

                    await _repository.SetTwoFactorDisabledAsync(user);

                    TempData["Redirect"] = RedirectPurpose.TwoFactorDisabled;
                    return RedirectToAction("Index");

                case TwoFactorAction.GetVerificationCodes:
                    if (!passwordValid || !twoFactorValid)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Credentials.");
                        return View(model);
                    }

                    TempData["Redirect"] = RedirectPurpose.VerificationCodes;
                    TempData["RecoveryCodes"] = JsonConvert.SerializeObject(await _repository.GenerateUserRecoveryCodesAsync(user));
                    return RedirectToAction("Index");
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
    }
}