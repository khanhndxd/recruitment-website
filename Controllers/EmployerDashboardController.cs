using DoanWebsiteTuyenDung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoanWebsiteTuyenDung.Controllers
{
    [Route("employer")]
    public class EmployerDashboardController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly DoanWebsiteTuyenDungContext _context;
        public EmployerDashboardController(DoanWebsiteTuyenDungContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _contextAccessor = httpContextAccessor;
        }

		[Route("{userid}/dashboard")]
		public async Task<IActionResult> Index()
        {
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            var employer = await _context.Employers.FindAsync(userid);
            if (employer == null)
            {
                return NotFound();
            }
            else
            {
                return View("Index", employer);
            }
        }

        [Route("{userid}/jobs")]
        public IActionResult Jobs()
        {
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            List<Job> jobs = _context.Jobs
                .Include(j => j.EIdNavigation)
                .Where(j => j.EIdNavigation.EId == userid)
                .ToList();
            return View("Jobs", jobs);
        }

        [HttpGet]
        [Route("{userid}/dashboard/edit/{jobId}")]
        public async Task<IActionResult> EditJob(string jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            DateTime expirationDate = job.JExpirationDate;
            DateTime postDate = job.JPostDate;
            TimeSpan difference = expirationDate.Subtract(postDate);
            int daysUntilExpiration = (int)difference.TotalDays;
            ViewData["daysUntilExpiration"] = daysUntilExpiration;
            return View(job);
        }


        [HttpPost]
        [Route("{userid}/dashboard/edit/{jobId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJob([FromRoute] string jobId, Job job)
        {
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            var olderJob = await _context.Jobs.FindAsync(jobId);
            string description = Request.Form["quillContent"];
            string required = Request.Form["quillContent-2"];
            int so_ngay_het_han = int.Parse(Request.Form["so-ngay-het-han"]);
            olderJob.JTitle = job.JTitle;
            olderJob.JLocation = job.JLocation;
            DateTime J_expirationDate = DateTime.Now.AddDays(so_ngay_het_han);
            olderJob.JType = job.JType;
            olderJob.JSalary = job.JSalary;
            olderJob.JExpirationDate = J_expirationDate;
            olderJob.JDescription = description;
            olderJob.JRequiredSkills = required;

            await _context.SaveChangesAsync();
            TempData["success"] = "Sửa tin tuyển dụng thành công";

            List<Job> jobs = _context.Jobs
                .Include(j => j.EIdNavigation)
                .Where(j => j.EIdNavigation.EId == userid)
                .ToList();
            return View("Jobs", jobs);
        }

        [HttpGet]
        [Route("{userid}/dashboard/update")]
        public async Task<IActionResult> UpdateEmployerInfo()
        {
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            var emp = await _context.Employers.FindAsync(userid);
            return View(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{userid}/dashboard/update")]
        public async Task<IActionResult> UpdateEmployerInfo(Employer emp)
        {
            string about = Request.Form["quillContent"];
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            var employer = await _context.Employers.FindAsync(userid);
            employer.EName = emp.EName;
            employer.EEmail = emp.EEmail;
            employer.EPhone = emp.EPhone;
            employer.EImage = emp.EImage;
            employer.EAddress = emp.EAddress;
            employer.EAbout = about;

            await _context.SaveChangesAsync();

			if (employer == null)
			{
				return NotFound();
			}
			else
			{
				return View("Index", employer);
			}
		}


        [HttpGet]
        [Route("{userid}/dashboard/delete/{jobId}")]
        public async Task<IActionResult> DeleteJob(string jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if(job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{userid}/dashboard/delete/{jobId}")]
        public async Task<IActionResult> DeleteJob([FromRoute] string jobId, Job job)
        {
            var j = await _context.Jobs.FindAsync(jobId);
            if (j != null)
            {
                _context.Jobs.Remove(j);
                await _context.SaveChangesAsync();
            }

            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            var employer = await _context.Employers.FindAsync(userid);
            if (employer == null)
            {
                return NotFound();
            }
            else
            {
                return View("Index", employer);
            }
        }
    }
}
