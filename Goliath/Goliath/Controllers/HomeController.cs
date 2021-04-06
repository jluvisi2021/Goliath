using Goliath.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Goliath.Controllers
{
    /*
     *
     * Main layout page for all requests.
     *  - Is the default page users see.
     *  - Can be redirected to login/register for authentication controller.
     *  - Shows a ContactUs page and a privacy page.
     *  - Index page has: Graphics, Description, FAQs.
     *
     *
     */

    [Route("")]
    public sealed class HomeController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public HomeController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            return View();
        }
    }
}