using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<object> settings = new List<object>();
            var fontAwesomeJS = _configuration["FontAwesome:js-file"];
            settings.AddRange(new object[] { fontAwesomeJS });
            ViewBag.Message = JsonConvert.SerializeObject(settings);
            return View();
        }
    }
}
