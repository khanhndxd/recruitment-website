﻿using DoanWebsiteTuyenDung.Models;
using Microsoft.AspNetCore.Http;
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

        [Route("{userid}/dashboard/postjob")]
        public IActionResult PostJobDashboard()
        {
            string usertype = _contextAccessor.HttpContext.Session.GetString("usertype");
            string userid = _contextAccessor.HttpContext.Session.GetString("userId");
            ViewData["userid"] = userid;
            if (usertype == "emp")
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
        [Route("{userid}/dashboard/postjob")]
        public async Task<IActionResult> PostJobDashboard(Job job)
        {
            string description = Request.Form["quillContent"];
            string required = Request.Form["quillContent-2"];
            int so_ngay_het_han = int.Parse(Request.Form["so-ngay-het-han"]);
            string Id;
            var lastRecord = await _context.Jobs.OrderByDescending(e => e.JId).FirstOrDefaultAsync();
            if (lastRecord == null)
            {
                Id = "";
            }
            else
            {
                Id = lastRecord.JId;
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
                newId = "job0" + nextId.ToString();
            }
            else
            {
                newId = "job" + nextId.ToString();
            }

            job.JId = newId;
            job.JDescription = description;
            job.JRequiredSkills = required;
            job.EId = _contextAccessor.HttpContext.Session.GetString("userId");
            job.JStatus = 0;
            DateTime J_expirationDate = DateTime.Now.AddDays(so_ngay_het_han);
            job.JExpirationDate = J_expirationDate;

            await _context.AddAsync(job);
            TempData["success"] = "Đăng tải công việc thành công";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { userid = _contextAccessor.HttpContext.Session.GetString("userId") });
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
            return RedirectToAction("Jobs", new
            {
                userid = userid
            });
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
                return RedirectToAction("Index", new
                {
                    userid = userid
                });
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
                return RedirectToAction("Index", new
                {
                    userid = userid
                });
            }
        }

        [HttpGet]
        [Route("{userid}/dashboard/{jobId}/jobseekers")]
        public async Task<IActionResult> GetJobSeeker([FromRoute] string jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            ViewData["jobTitle"] = job.JTitle;
            ViewData["jobId"] = jobId;

            var jobSeekerResumeList = _context.Applications
                .Where(a => a.JId == jobId)
                .Select(a => new { JobSeeker = a.RIds.Select(r => r.Js).FirstOrDefault(), Resume = a.RIds.FirstOrDefault() })
                .Cast<object>()
                .ToList();

            return View(jobSeekerResumeList);
        }

        [HttpGet]
        [Route("{userid}/dashboard/{jobId}/jobseekers/{jsId}/detail")]
        public async Task<IActionResult> JobSeekerDetail([FromRoute] string jsId, [FromRoute] string jobId)
        {
            var resume = _context.Applications
                .Where(a => a.JId == jobId && a.RIds.Any(r => r.JsId == jsId))
                .SelectMany(a => a.RIds)
                .Distinct()
                .FirstOrDefault();

            ViewData["Resume"] = resume;
            var jobSeeker = await _context.JobSeekers.FindAsync(jsId);
            return View(jobSeeker);
        }
    }
}
