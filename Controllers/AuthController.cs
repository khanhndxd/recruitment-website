using Microsoft.AspNetCore.Mvc;

namespace DoanWebsiteTuyenDung.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
