using Goliath.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    [ResponseCache(Duration = 14400, Location = ResponseCacheLocation.Any, NoStore = false)]
    public sealed class HomeController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger _logger;

        public HomeController(IAccountRepository accountRepository, ILogger<HomeController> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Arrived at Home Controller.");
            return View();
        }
    }
}