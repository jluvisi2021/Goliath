﻿using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using Goliath.Repository;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IAccountRepository _repository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ErrorsController(IAccountRepository repository, SignInManager<ApplicationUser> signInManager)
        {
            _repository = repository;
            _signInManager = signInManager;
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
                DateTime = DateTime.UtcNow.ToString() + " (UTC)",
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
            TempData["Redirect"] = RedirectPurpose.Exception;
            TempData["ErrorInformation"] = JsonConvert.SerializeObject(model);
            return RedirectToActionPermanent("Index", "Auth");
        }

        public IActionResult NoJS() => View();
    }
}