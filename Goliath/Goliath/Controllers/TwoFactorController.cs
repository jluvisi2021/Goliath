using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the routing/validation for two factor authentication.
    /// All two-factor authentication views are stored in userpanel.
    /// </summary>
    [Route("userpanel/2fa")]
    public class TwoFactorController : Controller
    {
        public TwoFactorController()
        {

        }

        /// <summary>
        /// The index view which displays the options for two factor.
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// View for setting up two factor authentication for a SMS verification.
        /// </summary>
        /// <returns></returns>
        [Route("sms")]
        public IActionResult SetupSms2FA()
        {
            return View();
        }

        /// <summary>
        /// View for setting up 2FA through a mobile app like Microsoft Authenticate.
        /// </summary>
        /// <returns></returns>
        [Route("app")]
        public IActionResult SetupApp2FA()
        {
            return View();
        }
    }
}
