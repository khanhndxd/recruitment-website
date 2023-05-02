using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoanWebsiteTuyenDung.Controllers
{
    public class SignoutController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public SignoutController(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            _contextAccessor.HttpContext.Session.Clear();
            TempData["success"] = "Đăng xuất thành công";
            return RedirectToAction("Index", "Home");
        }
    }
}
