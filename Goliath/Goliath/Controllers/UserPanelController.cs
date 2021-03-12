using Goliath.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    public sealed class UserPanelController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public UserPanelController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public IActionResult Index()
        {
            //ViewBag.config = _config.Serialize();
            return View("Profile");
        }

        /// <summary>
        /// Returns a specific partial view in Ajax.
        /// </summary>
        /// <param name="partialName"></param>
        /// <returns></returns>
        public ActionResult GetModule(string partialName)
        {
            return PartialView("~/Views/UserPanel/" + partialName + ".cshtml");
        }

        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.SignOutAsync();
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