using Goliath.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Goliath.Controllers
{
    public class RedirectController : Controller
    {
        [RequireHttps]
        public IActionResult Index(string redirectUrl, string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                string link = Url.ActionLink("Index", "Home");
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = $"<html><body>" +
                    $"You attempted to redirect from Goliath but your <strong>returnURL</strong> may have been malicious because it is not a valid Goliath URL.<br /><a href={link}>Click to return home.</a>" +
                    $"</body></html>"
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