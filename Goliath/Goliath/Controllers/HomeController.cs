using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goliath.Controllers
{

    /// <summary>
    /// Manages the Views for the Home.
    /// </summary>
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

       
    }
}
