using Microsoft.AspNetCore.Mvc;

namespace Goliath.Controllers
{
    /// <summary>
    /// Pops up a captcha prompt on a screen. Covers the previous view.
    /// </summary>
    public class CaptchaController : Controller
    {
        public IActionResult LoadCaptcha(string formID)
        {
            ViewData["FormID"] = formID;
            return PartialView("~/Views/Captcha/_ValidateCaptcha.cshtml");
        }
    }
}