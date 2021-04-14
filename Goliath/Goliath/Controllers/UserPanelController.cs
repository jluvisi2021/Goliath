﻿using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using Goliath.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the views for the User Panel which repersents the data that can be manipulated by a
    /// registered user.
    /// </summary>
    public sealed class UserPanelController : Controller
    {
        /// <summary>
        /// For interfacing with the ApplicationUser.
        /// </summary>
        private readonly IAccountRepository _accountRepository;

        public UserPanelController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public IActionResult Index()
        {
            return View("Profile");
        }

        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileSettingsGeneralModel model)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var goliathUser = await _accountRepository.GetUserByName(User.Identity.Name);
            // track if there has been any updates.
            bool hasChanged = false;

            if(hasValueChanged(model.BackgroundColor, goliathUser.BackgroundColor))
            {
                hasChanged = true;
                goliathUser.BackgroundColor = model.BackgroundColor;
            }
            if(hasValueChanged(model.DarkThemeEnabled, goliathUser.DarkTheme))
            {
                hasChanged = true;
                goliathUser.DarkTheme = model.DarkThemeEnabled;
            }
            if(hasValueChanged(model.NewEmail, goliathUser.Email))
            {
                hasChanged = true;
                goliathUser.Email = model.NewEmail;
            }
            if(hasValueChanged(model.NewPhoneNumber, goliathUser.PhoneNumber))
            {
                hasChanged = true;
                goliathUser.PhoneNumber = model.NewPhoneNumber;
            }
            if(hasChanged)
            {
                await _accountRepository.UpdateUser(goliathUser);
            }

            return View(model);
        }

        private bool hasValueChanged(string model, string user)
        {
            if(string.IsNullOrWhiteSpace(model))
            {
                GoliathHelper.PrintDebugger("No Changes detected");
                return false;
            }
            if(string.IsNullOrWhiteSpace(user) || user == "NULL")
            {
                return true;
            }
            if (!model.Equals(user))
            {
                return true;
            }
            GoliathHelper.PrintDebugger("No Changes detected");
            return false;
        }

        /// <summary>
        /// Returns a specific partial view in Ajax. <br /> Specifically useful for loading a part
        /// of the screen that is unknown at runtime.
        /// </summary>
        /// <param name="partialName"> File name of partial view. </param>
        /// <returns> </returns>
        public ActionResult GetModule(string partialName) => PartialView($"~/Views/UserPanel/{partialName}.cshtml");

        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.SignOutAsync();

            // Redirect the user to the main login screen with information that the user has just
            // been logged out.
            TempData["Redirect"] = RedirectPurpose.LogoutSuccess;
            return RedirectToAction("Index", "Auth");
        }

        public IActionResult Database() => View();

        public IActionResult Utilities() => View();

        [ResponseCache(Duration = 14400, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Help() => View();

        [ResponseCache(Duration = 14400, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult About() => View();

        [ResponseCache(Duration = 14400, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult BuildInfo() => View();

        public IActionResult AdminPanel() => View();
    }
}