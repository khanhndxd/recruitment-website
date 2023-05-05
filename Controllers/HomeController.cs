using DoanWebsiteTuyenDung.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DoanWebsiteTuyenDung.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult PostJob()
        {
            string usertype = HttpContext.Session.GetString("usertype");
            if(usertype == "emp")
            {
                return View();
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [Route("{username}/dashboard")]
        public IActionResult Dashboard(string username)
        {
			ViewData["username"] = username;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}