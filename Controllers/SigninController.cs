using DoanWebsiteTuyenDung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoanWebsiteTuyenDung.Controllers
{
    public class SigninController : Controller
    {
        private readonly DoanWebsiteTuyenDungContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public SigninController(DoanWebsiteTuyenDungContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
			_contextAccessor= httpContextAccessor;
		}
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Employer()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Employer([Bind("EEmail,EPassword")] Employer employer) 
        {
            var e = await _context.Employers.FirstOrDefaultAsync(e => e.EEmail == employer.EEmail && e.EPassword == employer.EPassword);
            if(e != null)
            {
                _contextAccessor.HttpContext.Session.SetString("username", e.EName);
                _contextAccessor.HttpContext.Session.SetString("email", e.EEmail);
				TempData["success"] = "Đăng nhập thành công";
				return RedirectToAction("Index", "Home");
            }
			TempData["error"] = "Đăng nhập không thành công";
			return RedirectToAction("Employer", "Signin"); 
        }
        public IActionResult JobSeeker()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobSeeker([Bind("JsEmail,JsPassword")] JobSeeker jobseeker)
        {
            var e = await _context.JobSeekers.FirstOrDefaultAsync(e => e.JsEmail == jobseeker.JsEmail && e.JsPassword == jobseeker.JsPassword);
            if (e != null)
            {
				_contextAccessor.HttpContext.Session.SetString("username", e.JsName);
				_contextAccessor.HttpContext.Session.SetString("email", e.JsName);
				TempData["success"] = "Đăng nhập thành công";
				return RedirectToAction("Index", "Home");
            }
			TempData["error"] = "Đăng nhập không thành công";
			return RedirectToAction("Employer", "Signin");
        }
    }
}
