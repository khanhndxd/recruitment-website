using DoanWebsiteTuyenDung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoanWebsiteTuyenDung.Controllers
{
    public class SignupController : Controller
    {
        private readonly DoanWebsiteTuyenDungContext _context;
        public SignupController(DoanWebsiteTuyenDungContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Employer([Bind("EName,EEmail,EPassword")] Employer employer)
        {
			string confirmPassword = Request.Form["ConfirmPassword"];
            string Id;
            if(employer.EPassword != confirmPassword)
            {
                TempData["error"] = "Mật khẩu nhập lại không chính xác";
                return RedirectToAction("Employer", "Signup");
            }
			var lastRecord = await _context.Employers.OrderByDescending(e => e.EId).FirstOrDefaultAsync();
            if (lastRecord == null)
            {
                Id = "";
            }
            else
            {
                Id = lastRecord.EId;
            }

            int nextId;
            string currentId = String.Join("", Id.Where(char.IsDigit));
            if (currentId == "")
            {
                nextId = 1;
            }
            else
            {
                nextId = Int32.Parse(currentId) + 1;
            }
            string newId;
            if(nextId <= 9)
            {
                newId = "e0" + nextId.ToString();
            } else
            {
                newId = "e" + nextId.ToString();
            }
            employer.EId = newId;
            await _context.AddAsync(employer);
            TempData["success"] = "Đăng ký thành công";
            await _context.SaveChangesAsync();
            return RedirectToAction("Employer", "Signin");
            //string errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            //ModelState.AddModelError("", errors);
        }
        public IActionResult JobSeeker() 
        { 
            return View(); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobSeeker([Bind("JsId, JsName,JsEmail,JsPassword")] JobSeeker jobseeker)
        {
            string confirmPassword = Request.Form["ConfirmPassword"];
            string Id;
            if (jobseeker.JsPassword != confirmPassword)
            {
                TempData["error"] = "Mật khẩu nhập lại không chính xác";
                return RedirectToAction("JobSeeker", "Signup");
            }
            var lastRecord = await _context.JobSeekers.OrderByDescending(e => e.JsId).FirstOrDefaultAsync();
            if(lastRecord == null)
            {
                Id = "";
            } else
            {
                Id = lastRecord.JsId;
            }
            int nextId;
            string currentId = String.Join("", Id.Where(char.IsDigit));
            if(currentId == "")
            {
                nextId = 1;
            }
            else
            {
                nextId = Int32.Parse(currentId) + 1;
            }
            string newId;
            if (nextId <= 9)
            {
                newId = "js0" + nextId.ToString();
            }
            else
            {
                newId = "js" + nextId.ToString();
            }
            jobseeker.JsId = newId;
            await _context.AddAsync(jobseeker);
            TempData["success"] = "Đăng ký thành công";
            await _context.SaveChangesAsync();
            return RedirectToAction("JobSeeker", "Signin");
            //string errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            //ModelState.AddModelError("", errors);
        }
    }
}
