using Goliath.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages bad requests on HTTPS. (TODO)
    /// </summary>
    public class ErrorsController : Controller
    {
        public ErrorsController()
        {
        }

        public IActionResult Index(string code)
        {
            BadStatusCodeModel model = new()
            {
                StatusCode = code,
                LastPageVisited = Request.Headers["Referer"],
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            return View(model);
        }

        public IActionResult NoJS()
        {
            return View();
        }
    }
}