using Goliath.Enums;
using Goliath.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Goliath.Controllers
{
    public class RedirectController : Controller
    {
        private readonly ILogger _logger;

        public RedirectController(ILogger<RedirectController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string redirectUrl, string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                string link = Url.ActionLink(nameof(HomeController.Index), GoliathControllers.HomeController);
                _logger.LogInformation($"Prevented malicious link {returnUrl} has been blocked. [redirectUrl = {redirectUrl}]");
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = $"You attempted to redirect from Goliath but your <strong>returnURL</strong> may have been malicious because it is not a valid Goliath URL.<br /><a href={link}>Click to return home.</a>"
                };
            }
            RedirectUrlModel model = new()
            {
                Url = redirectUrl,
                ReturnUrl = returnUrl
            };
            return View(model);
        }
    }
}