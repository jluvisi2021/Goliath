using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using Goliath.Repository;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages bad requests on HTTPS.
    /// </summary>
    // Do not store the status error pages or else it will not update.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public sealed class ErrorsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IAccountRepository _repository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ErrorsController(IAccountRepository repository, SignInManager<ApplicationUser> signInManager, ILogger<ErrorsController> logger)
        {
            _repository = repository;
            _signInManager = signInManager;
            _logger = logger;
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

        public async Task<IActionResult> GeneralException()
        {
            IExceptionHandlerPathFeature feature = HttpContext
              .Features
              .Get<IExceptionHandlerPathFeature>();
            ExceptionHandlerModel model = new()
            {
                DateTime = $"{DateTime.UtcNow} (UTC)",
                OriginalPath = feature?.Path ?? "Unknown",
                ExceptionSource = feature?.Error.Source ?? "Unknown",
                ExceptionTargetSite = feature?.Error.TargetSite.Name ?? "Unknown",
                ExceptionTargetHelpLink = feature?.Error.HelpLink ?? "Unknown",
                RawExceptionMessage = feature?.Error.Message ?? "Unknown",

                StatusCode = HttpContext.Response.StatusCode.ToString() ?? "Unknown"
            };
            GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, feature.Error.StackTrace);
            if (_signInManager.IsSignedIn(User))
            {
                await _repository.SignOutAsync();
            }
            _logger.LogError($"Encountered error in execution: {feature.Error.StackTrace}");
            TempData[TempDataKeys.Redirect] = RedirectPurpose.Exception;
            TempData[TempDataKeys.ErrorInformation] = JsonConvert.SerializeObject(model);
            return RedirectToActionPermanent(nameof(AuthController.Index), GoliathControllers.AuthController);
        }

        public IActionResult NoJS() => View();
    }
}