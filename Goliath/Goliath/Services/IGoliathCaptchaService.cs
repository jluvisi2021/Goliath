using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goliath.Services
{
    /// <summary>
    /// Manages validation of Captchas and their respective cookie hashes.
    /// </summary>
    public interface IGoliathCaptchaService
    {
        /// <summary>
        /// Generates a secure random number and then adds a <b> SHA-256 </b> hash of that number to
        /// the user's browser in the form of a cookie. <br /> Then adds that same number (not
        /// hashed) to the EF Core Table which manages captcha validation tokens.
        /// </summary>
        /// <returns> </returns>
        Task CacheNewCaptchaValidateAsync();

        /// <summary>
        /// Reads the <b> SHA-256 </b> hash inside of the Captcha cookie. <br /> That value is then
        /// searched for and compared against values in the captcha token table.
        /// </summary>
        /// <returns> If the cookie hash value is valid or the captcha has been completed successfully. </returns>
        Task<bool> IsCaptchaValidAsync();

        /// <summary>
        /// Checks if the Captcha is valid.
        /// </summary>
        /// <param name="useCookies"> Whether or not to search for the cookie. </param>
        /// <returns> If the cookie hash value is valid. </returns>
        Task<bool> IsCaptchaValidAsync(bool useCookies);

        /// <summary>
        /// Removes the Captcha cookie from the browser using <see cref="ICookieManager" />.
        /// </summary>
        void DeleteCaptchaCookie();

        /// <returns> The Key/Value pair for a ModelState Error. </returns>
        KeyValuePair<string, string> CaptchaValidationError();
    }
}