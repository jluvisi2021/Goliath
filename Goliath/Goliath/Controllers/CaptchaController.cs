using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace Goliath.Controllers
{
    /// <summary>
    /// Pops up a captcha prompt on a screen. Covers the previous view.
    /// </summary>
    public sealed class CaptchaController : Controller
    {
        public IActionResult LoadCaptcha(string formID)
        {
            ViewData["FormID"] = formID;
            return PartialView("~/Views/Captcha/_ValidateCaptcha.cshtml");
        }
    }
}