using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    
    public class UserPanelController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserPanelController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<object> settings = new List<object>();
            var fontAwesomeJS = _configuration["FontAwesome:js-file"];
            settings.AddRange(new object[] { fontAwesomeJS });
            ViewBag.Message = JsonConvert.SerializeObject(settings);
            return View("Profile");
        }

        public IActionResult Database()
        {
            return View();
        }

    }
}
