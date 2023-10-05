using Microsoft.AspNetCore.Mvc;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using Pink_Panthers_Project.Util;

namespace Pink_Panthers_Project.Controllers
{
    public class ClassController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;


        public ClassController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }



        public IActionResult Index(int id)
        {
            var account = HttpContext.Session.GetSessionValue<Account>("LoggedInAccount");
            ViewBag.isTeacher = account!.isTeacher;
            var currentClass = _context.Class.Find(id);

            if (currentClass == null)
            {
                return NotFound();
            }
            else
            {
                var assignments = _context.Assignments.Where(a => a.ClassID == id);

                var viewModel = new ClassViewModel
                {
                    Class = currentClass,
                    Assignments = assignments.ToList(),
                    Account = account
                };
                return View(viewModel);
            }
        }

        public IActionResult Create(int id)
        {
            var account = HttpContext.Session.GetSessionValue<Account>("LoggedInAccount");
            ViewBag.isTeacher = account!.isTeacher;
            ViewBag.ClassID = id;
            var currentClass = _context.Class.Find(id);

            if (currentClass == null)
            {
                return NotFound();
            } 
            else
            {
                ViewBag.ClassName = currentClass.CourseName;
                return View();
            }
           
			
		}

        [HttpPost]
        public IActionResult Create([Bind("ClassID,AssignmentName,DueDate,PossiblePoints,Description,SubmissionType")]Assignment assignement)
        {
            var account = HttpContext.Session.GetSessionValue<Account>("LoggedInAccount");
			ViewBag.isTeacher = account!.isTeacher;

            if (assignement.DueDate < DateTime.Now)
            {
				ModelState.AddModelError("DueDate", "Due date must be in the future");
			}

			if (ModelState.IsValid)
            {
                assignement.DueDate = assignement.DueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
				_context.Add(assignement);
				_context.SaveChanges();
				return RedirectToAction("Index", new { id = assignement.ClassID});
			}
            else
            {
				ViewBag.ClassID = assignement.ClassID;
				var currentClass = _context.Class.Find(assignement.ClassID);
				ViewBag.ClassName = currentClass?.CourseName;
                return View("Create", assignement);
			}
		}
    }
}
