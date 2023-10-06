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
		private static string session = "";
        private static Dictionary<string, Class?> _class = new Dictionary<string, Class?>();
        private static Dictionary<string, Account?> _account = new Dictionary<string, Account?>();
        private static bool isUnitTesting = false;

		public ClassController(Pink_Panthers_ProjectContext context, bool unitTest = false)
        {
            _context = context;
            if (unitTest)
                isUnitTesting = true;
        }


        public IActionResult Index()
        {
			session = isUnitTesting ? "test" : HttpContext.Session.Id; //Much cleaner than if(isUnitTesting) { } else { }
            setAccount();

			List<Class> tCourses = new List<Class>();
            List<Class> sCourses = new List<Class>();

			if (_account[session]! == null)
            {
                return NotFound();
            }
            ViewBag.isTeacher = _account[session]!.isTeacher;

            if (_account[session]!.isTeacher)
            {
                tCourses = _context.Class.Where(c => c.accountID == _account[session]!.ID).ToList();
            }
            else if (!_account[session]!.isTeacher)
            {
                sCourses = _context.registeredClasses.Where(rc => rc.accountID == _account[session]!.ID)
                    .Join(_context.Class, rc => rc.classID, c => c.ID, (rc, c) => new Class
                    {
                        ID = c.ID,
                        DepartmentCode = c.DepartmentCode,
                        CourseNumber = c.CourseNumber,
                        CourseName = c.CourseName,
                        Room = c.Room,
                        StartTime = c.StartTime,
                        EndTime = c.EndTime,
                        Days = c.Days,
                        tName = _context.Account.Where(t => t.ID == c.accountID).Select(n => n.FirstName + " " + n.LastName).SingleOrDefault(),
                        color = c.color,
                        hours = c.hours
                    }).ToList();
            }
            

            var viewModel = new CourseView
            {
                TeachingCourses = tCourses,
                RegisteredCourses = sCourses,
                Account = _account[session]!
            };
            return View(viewModel);
        }

        public IActionResult Assignments(int? id)
        {
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

			if (_account[session]! == null)
            {
                return NotFound();
            }
			setClass(id);
            if(_class[session]! == null)
            {
                return NotFound();
            }

			ViewBag.isTeacher = _account[session]!.isTeacher;

			var assignments = _context.Assignments.Where(a => a.ClassID == _class[session]!.ID);

			var viewModel = new ClassViewModel
			{
				Class = _class[session]!,
				Assignments = assignments.ToList(),
				Account = _account[session]!
			};
			return View(viewModel);
		}

        [HttpGet]
        public IActionResult GradeAssignment(int? id)
        {
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

			if (_account[session]! == null)
                return NotFound();

            ViewBag.isTeacher = _account[session]!.isTeacher;
            //else
            if (!_account[session]!.isTeacher)
            {
                return NotFound();
            }

            StudentSubmission sSub;
            sSub = _context.StudentSubmissions.Find(id)!;
            sSub.studentAccount = _context.Account.Find(sSub.AccountID);
            sSub.currentAssignment = _context.Assignments.Find(sSub.AssignmentID);

            return View(sSub);
        }

		[HttpPost]
		public async Task<IActionResult> GradeAssignment([Bind("ID,Grade")]StudentSubmission newGrade)
		{
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

			if (_account[session]! == null)
				return NotFound();

			//else
			StudentSubmission sSub;
			sSub = _context.StudentSubmissions.Find(newGrade.ID)!;
			if(newGrade.Grade != null)
            {
                sSub.Grade = newGrade.Grade;
                _context.StudentSubmissions.Update(sSub);
                await _context.SaveChangesAsync();
			    return RedirectToAction("ViewSubmissions", new { assignmentID = sSub.AssignmentID } );
            }

            ModelState.AddModelError("GradeMustBeEntered", "");
            return View(sSub);
		}

		[HttpGet]
        public IActionResult Create()
        {
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

			ViewBag.isTeacher = _account[session]!.isTeacher;
            ViewBag.ClassID = _class[session]!.ID;

            if (_class[session]! == null)
            {
                return NotFound();
            } 
            else
            {
                ViewBag.ClassName = _class[session]!.CourseName;
                return View();
            }
           
			
		}

        [HttpPost]
        public IActionResult Create([Bind("ClassID,AssignmentName,DueDate,PossiblePoints,Description,SubmissionType")]Assignment assignment)
        {
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

			ViewBag.isTeacher = _account[session]!.isTeacher;

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
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

            int classID = _context.Assignments.Where(c => c.Id == assignmentID).Select(c => c.ClassID).SingleOrDefault();
            _class[session] = _context.Class.Where(c => c.ID == classID).SingleOrDefault();

            return RedirectToAction("SubmitAssignment", new { assignmentID = assignmentID });
        }

        [HttpGet]
        public IActionResult SubmitAssignment(int assignmentID)
        {
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

			ViewBag.isTeacher = _account[session]!.isTeacher;

			if (_class[session]! == null)
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
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

			if (ModelState.IsValid)
            {
                newSubmission!.AccountID = _account[session]!.ID;
                if (await _context.StudentSubmissions.Where(c => c.AccountID == newSubmission.AccountID && c.AssignmentID == newSubmission.AssignmentID).SingleOrDefaultAsync() == null)
                {
                    await _context.StudentSubmissions.AddAsync(newSubmission!);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    StudentSubmission sub = await _context.StudentSubmissions.Where(c => c.AccountID == _account[session]!.ID && c.AssignmentID == newSubmission.AssignmentID).SingleAsync();
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
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

			if (_account[session]! == null || _class[session]! == null || !_account[session]!.isTeacher)
            {
                return NotFound();
            }
            ViewBag.isTeacher = _account[session]!.isTeacher;
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

        private void setClass(int? id)
        {
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

            _class[session] = _context.Class.Find(id);
		}

		public void setAccount(Account? account = null) //Used to set the current account
		{
			session = isUnitTesting ? "test" : HttpContext.Session.Id;

			_account![session] = isUnitTesting ? account! : HttpContext.Session.GetSessionValue<Account>("LoggedInAccount")!;

		}
	}
}
