using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// The controller for managing AES actions. 
    /// Currently used to decrypt user data.
    /// </summary>
    [Route("aes")]
    public class GoliathAesController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("decrypt")]
        public IActionResult DecryptUserData()
        {
            return View();
        }
    }
}
