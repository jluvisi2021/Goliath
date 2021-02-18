
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
