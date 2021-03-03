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

        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.SignOutAsync();
            return RedirectToAction("Index", "UserPanel");
        }

        public IActionResult Database()
        {
            //ViewBag.config = _config.Serialize();
            return View();
        }
    }
}