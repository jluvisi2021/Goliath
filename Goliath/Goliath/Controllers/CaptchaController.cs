using Microsoft.AspNetCore.Mvc;

namespace Goliath.Controllers
{
    /// <summary>
    /// Pops up a captcha prompt on a screen. Covers the previous view.
    /// </summary>
    public sealed class CaptchaController : Controller
    {
        private const string CaptchaModalPath = "~/Views/Captcha/_ValidateCaptcha.cshtml";

        public IActionResult LoadCaptcha(string formID)
        {
            ViewData["FormID"] = formID;
            return PartialView(CaptchaModalPath);
        }
    }
}