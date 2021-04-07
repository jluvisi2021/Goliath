using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goliath.Services
{
    public interface IGoliathCaptchaService
    {
        Task CacheNewCaptchaValidateAsync();

        Task<bool> IsCaptchaValidAsync();

        Task<bool> IsCaptchaValidAsync(bool useCookies);

        void DeleteCaptchaCookie();

        KeyValuePair<string, string> CaptchaValidationError();
    }
}