using Goliath.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the views for the User Panel which repersents the data that can
    /// be manipulated by a registered user.
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

        /// <summary>
        /// Returns a specific partial view in Ajax.
        /// <br />
        /// Specifically useful for loading a part of the screen that is unknown at runtime.
        /// </summary>
        /// <param name="partialName">File name of partial view.</param>
        /// <returns></returns>
        public ActionResult GetModule(string partialName)
        {
            return PartialView("~/Views/UserPanel/" + partialName + ".cshtml");
        }

        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.SignOutAsync();
            // Redirect the user to the main login screen with information that the user has just been logged out.
            TempData["Redirect"] = "logout";
            return RedirectToAction("Index", "Auth");
        }

        public IActionResult Database()
        {
            return View();
        }

        public IActionResult Utilities()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult BuildInfo()
        {
            return View();
        }
    }
}