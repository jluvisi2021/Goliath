using Goliath.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
