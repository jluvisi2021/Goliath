using Goliath.Helper;
using Goliath.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    public class RedirectController : Controller
    {
        [RequireHttps]
        public IActionResult Index(string redirectUrl, string returnUrl)
        {
            if(!Url.IsLocalUrl(returnUrl))
            {
                return Content("You attempted to redirect from Goliath but your returnURL may have been malicious because it is not a valid Goliath URL.");
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
