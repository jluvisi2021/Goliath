using Goliath.Enums;
using Goliath.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Goliath.Controllers
{
    public class RedirectController : Controller
    {
        [HttpGet]
        public IActionResult Index(string redirectUrl, string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                string link = Url.ActionLink(nameof(HomeController.Index), GoliathControllers.HomeController);
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