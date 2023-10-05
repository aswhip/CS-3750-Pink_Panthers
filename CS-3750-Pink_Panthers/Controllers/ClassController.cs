using Microsoft.AspNetCore.Mvc;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using Pink_Panthers_Project.Util;
using Pink_Panthers_Project.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Pink_Panthers_Project.Controllers
{
    public class ClassController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;


        public ClassController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }

        private static Class? _class;
        private static Account? _account; 

        public IActionResult Index(int? id)
        {
            if(_account == null)
                _account = ProfileController.getAccount();
            ViewBag.isTeacher = _account!.isTeacher;
            if(_class == null)
                _class = _context.Class.Find(id);

            var assignments = _context.Assignments.Where(a => a.ClassID == _class!.ID);

            var viewModel = new ClassViewModel
            {
                Class = _class,
                Assignments = assignments.ToList(),
                Account = _account
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.isTeacher = _account!.isTeacher;
            ViewBag.ClassID = _class!.ID;

            if (_class == null)
            {
                return NotFound();
            } 
            else
            {
                ViewBag.ClassName = _class.CourseName;
                return View();
            }
           
			
		}

        [HttpPost]
        public IActionResult Create([Bind("ClassID,AssignmentName,DueDate,PossiblePoints,Description,SubmissionType")]Assignment assignment)
        {
			ViewBag.isTeacher = _account!.isTeacher;

            if (assignment.DueDate < DateTime.Now)
            {
				ModelState.AddModelError("DueDate", "Due date must be in the future");
			}

			if (ModelState.IsValid)
            {
                assignment.DueDate = assignment.DueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
				_context.Add(assignment);
				_context.SaveChanges();
				return RedirectToAction("Index", new { id = assignment.ClassID});
			}
            else
            {
				ViewBag.ClassID = assignment.ClassID;
				var currentClass = _context.Class.Find(assignment.ClassID);
				ViewBag.ClassName = currentClass?.CourseName;
                return View("Create", assignment);
			}
		}

        [HttpGet]
        public IActionResult ToDoListClick(int assignmentID)
        {
            _account = ProfileController.getAccount();
            int classID = _context.Assignments.Where(c => c.Id == assignmentID).Select(c => c.ClassID).SingleOrDefault();
            _class = _context.Class.Where(c => c.ID == classID).SingleOrDefault();

            return RedirectToAction("SubmitAssignment", new { assignmentID = assignmentID });
        }

        [HttpGet]
        public IActionResult SubmitAssignment(int assignmentID)
        {
			ViewBag.isTeacher = _account!.isTeacher;

			if (_class == null)
			{
				return NotFound();
			}
			else
			{
                var submissionView = new StudentSubmission
                {
                    AssignmentID = assignmentID,
                    currentAssignment = _context.Assignments.Find(assignmentID)
                };
				return View(submissionView);
			}
		}
		[HttpPost]
		public async Task<IActionResult> SubmitAssignment(StudentSubmission? newSubmission)
		{
            if (ModelState.IsValid)
            {
                newSubmission!.AccountID = _account!.ID;
                if (await _context.StudentSubmissions.Where(c => c.AccountID == newSubmission.AccountID && c.AssignmentID == newSubmission.AssignmentID).SingleOrDefaultAsync() == null)
                {
                    await _context.StudentSubmissions.AddAsync(newSubmission!);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    StudentSubmission sub = await _context.StudentSubmissions.Where(c => c.AccountID == _account!.ID && c.AssignmentID == newSubmission.AssignmentID).SingleAsync();
                    sub.Submission = newSubmission.Submission;
                    _context.StudentSubmissions.Update(sub);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
		}

        [HttpGet]
        public IActionResult ViewSubmissions(int assignmentID)
        {
            if(_account == null || _class == null || !_account.isTeacher)
            {
                return NotFound();
            }
            var viewSubmissions = new SubmissionsViewModel
            {
                StudentSubmissions = _context.StudentSubmissions.Where(c => c.AssignmentID == assignmentID).ToList(),
                AssignmentName = _context.Assignments.Where(c => c.Id == assignmentID).Select(c => c.AssignmentName).SingleOrDefault()
            };
            foreach(var s in viewSubmissions.StudentSubmissions)
            {
                s.studentAccount = _context.Account.Where(c => c.ID == s.AccountID).SingleOrDefault();
                s.currentAssignment = _context.Assignments.Where(c => c.Id == s.AssignmentID).SingleOrDefault();
            }
            return View(viewSubmissions);
        }

        public static void resetClass()
        {
            _class = null;
        }
	}
}
