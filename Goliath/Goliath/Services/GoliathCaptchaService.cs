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
    /// <inheritdoc cref="IGoliathCaptchaService" />
    public class GoliathCaptchaService : IGoliathCaptchaService
    {
        private readonly IDNTCaptchaValidatorService _validatorService;
        private readonly DNTCaptchaOptions _captchaOptions;
        private readonly ICookieManager _cookieManager;
        private readonly IValidHumanVerifyTokensRepository _validTokens;

        public GoliathCaptchaService
            (IDNTCaptchaValidatorService validatorService,
            IOptions<DNTCaptchaOptions> options,
            ICookieManager cookieManager,
            IValidHumanVerifyTokensRepository validTokens)
        {
            _validatorService = validatorService;
            _captchaOptions = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;
            _cookieManager = cookieManager;
            _validTokens = validTokens;
        }

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
            await _validTokens.CleanUpUnusedTokensAsync();
            // Captcha completed successfully by user.
            return true;
        }

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

        public void DeleteCaptchaCookie()
        {
            _cookieManager.DeleteCookie(CookieKeys.ValidateCaptchaCookie);
        }

        public KeyValuePair<string, string> CaptchaValidationError()
        {
            return new KeyValuePair<string, string>(
                _captchaOptions.CaptchaComponent.CaptchaInputName, "Incorrect CAPTCHA."
            );
        }
    }
}