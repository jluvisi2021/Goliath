using DNTCaptcha.Core;
using Goliath.Enums;
using Goliath.Helper;
using Goliath.Repository;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goliath.Services
{
    /// <summary>
    /// Manages all CAPTCHAs and validations on Goliath.
    /// </summary>
    public class GoliathCaptchaService : IGoliathCaptchaService
    {
        private readonly IDNTCaptchaValidatorService _validatorService;
        private readonly DNTCaptchaOptions _captchaOptions;
        private readonly ICookieManager _cookieManager;
        private readonly IValidHumanVerifyTokensRepository _validTokens;

        public GoliathCaptchaService
            (
            IDNTCaptchaValidatorService validatorService,
            IOptions<DNTCaptchaOptions> options,
            ICookieManager cookieManager,
            IValidHumanVerifyTokensRepository validTokens
            )
        {
            _validatorService = validatorService;
            _captchaOptions = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;
            _cookieManager = cookieManager;
            _validTokens = validTokens;
        }

        /// <summary>
        /// Returns whether or not the captcha is valid and the user can perform a task.
        /// </summary>
        /// <returns> </returns>
        public async Task<bool> IsCaptchaValidAsync()
        {
            if (!(_validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.ShowDigits)))
            {
                // If the user has a captcha cookie.
                if (_cookieManager.HasCookie(CookieKeys.ValidateCaptchaCookie))
                {
                    bool validToken = await _validTokens.DoesTokenExistAsync(_cookieManager.CookieValue(CookieKeys.ValidateCaptchaCookie));
                    if (!validToken)
                    {
                        // Has a Cached Cookie but an invalid Hash.
                        return false;
                    }
                    // Valid Cookie with a Valid Hash (Cache Hit)
                    return true;
                }
                else
                {
                    // No Cached Cookie & Wrong Captcha Code.
                    return false;
                }
            }
            await _validTokens.CleanUpUnusedTokens();
            // Captcha completed successfully by user.
            return true;
        }

        /// <summary>
        /// Returns whether or not the captcha is valid and the user can perform a task.
        /// </summary>
        /// <param name="useCookies"> Should the captcha check look for a cached cookie? </param>
        /// <returns> </returns>
        public async Task<bool> IsCaptchaValidAsync(bool useCookies)
        {
            if (!(_validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.ShowDigits)))
            {
                if (!useCookies)
                {
                    return false;
                }
                // If the user has a captcha cookie.
                if (_cookieManager.HasCookie(CookieKeys.ValidateCaptchaCookie))
                {
                    bool validToken = await _validTokens.DoesTokenExistAsync(_cookieManager.CookieValue(CookieKeys.ValidateCaptchaCookie));
                    if (!validToken)
                    {
                        // Has a Cached Cookie but an invalid Hash.
                        return false;
                    }
                    // Valid Cookie with a Valid Hash (Cache Hit)
                    return true;
                }
                else
                {
                    // No Cached Cookie & Wrong Captcha Code.
                    return false;
                }
            }
            // Captcha completed successfully by user.
            return true;
        }

        /// <summary>
        /// A method which instructs Goliath to cache a successful captcha completion <br /> result
        /// in the database and add a cookie to the user's browser.
        /// </summary>
        /// <returns> </returns>
        public async Task CacheNewCaptchaValidateAsync()
        {
            string token = GoliathHelper.GenerateSecureRandomNumber();
            _cookieManager.AddCookie(
                key: CookieKeys.ValidateCaptchaCookie, // Name of the key.
                value: GoliathHash.HashStringSHA256(token), // A hash derived from token.
                expireTime: DateTime.UtcNow.AddMinutes(5) // Expires in 5 minutes.
                );
            // Add the generated random number to the database.
            await _validTokens.AddTokenAsync(key: token);
        }

        /// <summary>
        /// Removes a captcha cookie from the browser.
        /// </summary>
        public void DeleteCaptchaCookie()
        {
            _cookieManager.DeleteCookie(CookieKeys.ValidateCaptchaCookie);
        }

        /// <summary>
        /// A model error for views which have asp-validation.
        /// </summary>
        /// <returns> </returns>
        public KeyValuePair<string, string> CaptchaValidationError()
        {
            return new KeyValuePair<string, string>(
                _captchaOptions.CaptchaComponent.CaptchaInputName, "Incorrect CAPTCHA."
            );
        }
    }
}