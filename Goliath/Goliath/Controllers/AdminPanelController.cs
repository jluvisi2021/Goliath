using Microsoft.AspNetCore.Mvc;

namespace Goliath.Controllers
{
    public sealed class AdminPanelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}