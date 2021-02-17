using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    
    public class UserPanelController : Controller
    {
        
        public IActionResult Index()
        {
            return View("Profile");
        }

        public IActionResult Database()
        {
            return View();
        }

    }
}
