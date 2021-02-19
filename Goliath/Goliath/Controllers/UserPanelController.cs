using Microsoft.AspNetCore.Mvc;

namespace Goliath.Controllers
{
    public sealed class UserPanelController : Controller
    {
        public UserPanelController()
        {
        }

        public IActionResult Index()
        {
            //ViewBag.config = _config.Serialize();
            return View("Profile");
        }

        public IActionResult Database()
        {
            //ViewBag.config = _config.Serialize();
            return View();
        }
    }
}