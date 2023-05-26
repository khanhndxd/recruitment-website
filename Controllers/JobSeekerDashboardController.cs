using DoanWebsiteTuyenDung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DoanWebsiteTuyenDung.Controllers
{
    [Route("jobseeker")]
    public class JobSeekerDashboardController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly DoanWebsiteTuyenDungContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public JobSeekerDashboardController(DoanWebsiteTuyenDungContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _contextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }
        [Route("{userid}/dashboard")]
        public async Task<IActionResult> Index()
        {
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            var jobseeker = await _context.JobSeekers.FindAsync(userid);
            if (jobseeker == null)
            {
                return NotFound();
            }
            else
            {
                return View("Index", jobseeker);
            }
        }

        [HttpGet]
        [Route("{userid}/dashboard/update")]
        public async Task<IActionResult> UpdateJobSeekerInfo()
        {
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            var jsk = await _context.JobSeekers.FindAsync(userid);
            return View(jsk);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{userid}/dashboard/update")]
        public async Task<IActionResult> UpdateJobSeekerInfo(JobSeeker jsk)
        {
            string skills = Request.Form["quillContent"];
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            var jobseeker = await _context.JobSeekers.FindAsync(userid);
            jobseeker.JsName = jsk.JsName;
            jobseeker.JsEmail = jsk.JsEmail;
            jobseeker.JsPhone = jsk.JsPhone;
            jobseeker.JsImage = jsk.JsImage;
            jobseeker.JsAddress = jsk.JsAddress;
            jobseeker.JsSkills = skills;

            await _context.SaveChangesAsync();

            if (jobseeker == null)
            {
                return NotFound();
            }
            else
            {
                return RedirectToAction("Index", new { userid = userid });
            }
        }
        [Route("{userid}/resumes")]
        public IActionResult Resumes()
        {
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            List<Resume> res = _context.Resumes
                .Include(r => r.Js)
                .Where(r => r.Js.JsId == userid)
                .ToList();
            return View("Resumes", res);
        }

        [HttpGet]
        [Route("{userid}/resume/{reId}")]
        public IActionResult GetResumeDetail(string reId)
        {
            var re = _context.Resumes.Include(r => r.Js).FirstOrDefault(r => r.RId == reId);
            if (re == null)
            {
                return NotFound();
            }
            return View(re);
        }

        [Route("{userid}/resumes/create")]
        public IActionResult CreateResume()
        {
            string usertype = _contextAccessor.HttpContext.Session.GetString("usertype");
            if (usertype == "jsk")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{userid}/resumes/create")]
        public async Task<IActionResult> CreateResume(Resume resume)
        {
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            string content = Request.Form["quillContent"];
            var file = Request.Form.Files["file"];

            // Tạo thư mục nếu nó chưa tồn tại
            var uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "resumes");
            Directory.CreateDirectory(uploadFolder);

            // Lưu tệp được tải lên vào thư mục
            var filePath = Path.Combine(uploadFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var serverPath = "/resumes/" + file.FileName;

            string Id;
            var lastRecord = await _context.Resumes.OrderByDescending(r => r.RId).FirstOrDefaultAsync();
            if (lastRecord == null)
            {
                Id = "";
            }
            else
            {
                Id = lastRecord.RId;
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
            if (nextId <= 9)
            {
                newId = "re0" + nextId.ToString();
            }
            else
            {
                newId = "re" + nextId.ToString();
            }

            resume.RId = newId;
            resume.RContent = content;
            resume.JsId = userid;
            resume.RFilePath = serverPath;
            resume.RFileName = file.FileName;

            TempData["success"] = "Tạo sơ yếu lý lịch thành công";
            await _context.AddAsync(resume);
            await _context.SaveChangesAsync();

            var jobseeker = await _context.JobSeekers.FindAsync(userid);
            return View("Index", jobseeker);
        }

        [HttpGet]
        [Route("{userid}/dashboard/resumes/edit/{reId}")]
        public async Task<IActionResult> EditResume(string reId)
        {
            var re = await _context.Resumes.FindAsync(reId);
            return View(re);
        }


        [HttpPost]
        [Route("{userid}/dashboard/resumes/edit/{reId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResume([FromRoute] string reId, Resume resume)
        {
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            var olderRe = await _context.Resumes.FindAsync(reId);
            string content = Request.Form["quillContent"];
            olderRe.RName = resume.RName;
            olderRe.RContent = content;

            TempData["success"] = "Sửa hồ sơ thành công";
            await _context.SaveChangesAsync();

            List<Resume> res = _context.Resumes
                .Include(r => r.Js)
                .Where(r => r.Js.JsId == userid)
                .ToList();
            return RedirectToAction("Resumes", new { userid = userid });
        }

        [HttpGet]
        [Route("{userid}/dashboard/resumes/delete/{reId}")]
        public async Task<IActionResult> DeleteResume(string reId)
        {
            var re = await _context.Resumes.FindAsync(reId);
            if (re == null)
            {
                return NotFound();
            }
            return View(re);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{userid}/dashboard/resumes/delete/{reId}")]
        public async Task<IActionResult> DeleteResume([FromRoute] string reId, Resume resume)
        {
            var re = await _context.Resumes.FindAsync(reId);
            if (re != null)
            {
                _context.Resumes.Remove(re);
                await _context.SaveChangesAsync();
            }

            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            List<Resume> res = _context.Resumes
                .Include(r => r.Js)
                .Where(r => r.Js.JsId == userid)
                .ToList();
            return RedirectToAction("Resumes", new { userid = userid });
        }

        [HttpGet]
        [Route("{userid}/dashboard/applied-job")]
        public IActionResult AppliedJob([FromRoute] string userid)
        {
			string jobSeekerId = userid;
			List<Job> jobs = _context.JobSeekers
				.Where(js => js.JsId == jobSeekerId)
				.SelectMany(js => js.Resumes)
				.SelectMany(r => r.AIds)
				.Select(a => a.JIdNavigation)
                .Distinct()
                .ToList();
			return View(jobs);
		}
    }
}
