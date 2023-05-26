using DoanWebsiteTuyenDung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DoanWebsiteTuyenDung.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly DoanWebsiteTuyenDungContext _context;
        public HomeController(ILogger<HomeController> logger, DoanWebsiteTuyenDungContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _contextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            List<Job> jobs = _context.Jobs.Include(j => j.EIdNavigation).ToList(); 
            return View("Index", jobs);
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string name, string jobLocation, string jobType)
        {

            List<Job> jobs = _context.Jobs.Include(j => j.EIdNavigation)
                .Where(j => (name == null || j.EIdNavigation.EName.ToLower().Trim() == name.ToLower().Trim() || j.JTitle.ToLower().Trim().Contains(name.ToLower().Trim())) &&
                    (jobLocation == null || j.JLocation.ToLower().Trim() == jobLocation.ToLower().Trim()) &&
                    (jobType == null || j.JType.ToLower().Trim() == jobType.ToLower().Trim()))
                .ToList();

            return View("Index", jobs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult PostJob()
        {
            string usertype = _contextAccessor.HttpContext.Session.GetString("usertype");
            if(usertype == "emp")
            {
                return View();
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJob(Job job)
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
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("job/{jobId}")]
        public IActionResult GetJobDetail(string jobId)
        {
            var job = _context.Jobs.Include(j =>j.EIdNavigation).FirstOrDefault(j => j.JId == jobId);
            var numberFormatInfo = new NumberFormatInfo { NumberGroupSeparator = "." };
            string formattedSalary = job.JSalary.ToString("N0", numberFormatInfo) + " đ";
            ViewData["FormattedSalary"] = formattedSalary;
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        [HttpGet]
        [Route("employer/{eId}/detail")]
        public async Task<IActionResult> EmployerDetail([FromRoute] string eId)
        {
            var employer = await _context.Employers.FindAsync(eId);
            return View(employer);
        }

        [HttpGet]
        [Route("{userid}/apply/{jobid}")]
        public async Task<IActionResult> ApplyJob([FromRoute] string jobid)
        {
            var userid = _contextAccessor.HttpContext.Session.GetString("userId");
            ViewData["jobid"] = jobid;
            var resume = _context.Resumes.Include(r => r.Js).Where(r => r.Js.JsId == userid).ToList();
            if(resume != null)
            {
                return View(resume);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{userid}/apply/{jobid}")]
        public async Task<IActionResult> ApplyJob([FromRoute] string userid, [FromRoute] string jobid, [FromForm] string resume)
        {
            
            var jsk = await _context.JobSeekers.FindAsync(userid);
            var job = await _context.Jobs.FindAsync(jobid);
            var selectionResume = await _context.Resumes.FindAsync(resume);
            if(selectionResume == null)
            {
                TempData["error"] = "Không thể tìm thấy hồ sơ";
                return RedirectToAction("Index", "Home");
            }
            if (jsk != null && job != null)
            {
				string Id;
				var lastRecord = await _context.Applications.OrderByDescending(a => a.AId).FirstOrDefaultAsync();
				if (lastRecord == null)
				{
					Id = "";
				}
				else
				{
					Id = lastRecord.AId;
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
					newId = "appl0" + nextId.ToString();
				}
				else
				{
					newId = "appl" + nextId.ToString();
				}
				var application = new Application { JId = job.JId, AStatus = 0 };
				application.AId = newId;
                application.RIds.Add(selectionResume);
                job.JStatus = 1;
				TempData["success"] = "Apply công việc thành công";

				await _context.Applications.AddAsync(application);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return NotFound();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}