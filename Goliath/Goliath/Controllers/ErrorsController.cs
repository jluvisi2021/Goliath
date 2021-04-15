using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages bad requests on HTTPS.
    /// </summary>
    // Do not store the status error pages or else it will not update.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public sealed class ErrorsController : Controller
    {
        public ErrorsController()
        {
        }

        public IActionResult Index(string code)
        {
            BadStatusCodeModel model = new()
            {
                StatusCode = code,
                LastPageVisited = Request.Headers["Referer"],
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            return View(model);
        }

        public IActionResult GeneralException()
        {
            IExceptionHandlerPathFeature feature = HttpContext
              .Features
              .Get<IExceptionHandlerPathFeature>();
            ExceptionHandlerModel model = new()
            {
                DateTime = DateTime.Now.ToString(),
                OriginalPath = feature?.Path ?? "Unknown",
                ExceptionSource = feature?.Error.Source ?? "Unknown",
                ExceptionTargetSite = feature?.Error.TargetSite.Name ?? "Unknown",
                ExceptionTargetHelpLink = feature?.Error.HelpLink ?? "Unknown",
                RawExceptionMessage = feature?.Error.Message ?? "Unknown",

                StatusCode = HttpContext.Response.StatusCode.ToString() ?? "Unknown"
            };
            GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, feature.Error.StackTrace);
            TempData["Redirect"] = RedirectPurpose.Exception;
            TempData["ErrorInformation"] = JsonConvert.SerializeObject(model);
            return RedirectToActionPermanent("Index", "Auth");
        }

        public IActionResult NoJS() => View();
    }
}