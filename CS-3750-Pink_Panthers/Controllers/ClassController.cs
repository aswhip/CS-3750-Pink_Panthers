﻿using Microsoft.AspNetCore.Mvc;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using Pink_Panthers_Project.Util;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Pink_Panthers_Project.Controllers
{
    public class classController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;

		public classController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var account = getAccount();

			List<Class> tCourses = new List<Class>();
            List<Class> sCourses = new List<Class>();

			if (account == null)
            {
                return NotFound();
            }
            ViewBag.isTeacher = account!.isTeacher;

            if (account!.isTeacher)
            {
                tCourses = getTeacherClasses()!;
            }
            else if (!account!.isTeacher)
            {
                sCourses = getStudentClasses()!;
            }
            
            var viewModel = new CourseView
            {
                TeachingCourses = tCourses,
                RegisteredCourses = sCourses,
                Account = account!,
                UpcomingAssignments = getUpcomingAssignments()
            };
            return View(viewModel);
        }

        public IActionResult Assignments(int? id)
        {
            var account = getAccount();
			var cls = id != null ? getClass(id) : getClass(); //Gets the class based on id if id != null, otherwise just gets the current active class

            if (account! == null)
            {
                return NotFound();
            }
            if(cls! == null)
            {
                return NotFound();
            }

            ViewBag.Class = cls;
			ViewBag.isTeacher = account!.isTeacher;

            UpdateAssignments();
            var assignments = getAssignments();

            var submissions = getStudentSubmissions();

            var viewModel = new ClassViewModel
            {
                Class = cls!,
                Assignments = assignments!,
                Account = account!,
                StudentSubmissions = submissions,
                UpcomingAssignments = getUpcomingAssignments()
			};
			return View(viewModel);
		}

        [HttpGet]
        public IActionResult GradeAssignment(int? id)
        {
            var account = getAccount();

			if (account! == null)
                return NotFound();

            ViewBag.isTeacher = account!.isTeacher;
            //else
            if (!account!.isTeacher)
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
            var account = getAccount();

			if (account! == null)
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
        public IActionResult CreateAssignment()
        {
            var account = getAccount();
            var cls = getClass();

			ViewBag.isTeacher = account!.isTeacher;
            ViewBag.ClassID = cls!.ID;

            if (cls! == null)
            {
                return NotFound();
            } 
            else
            {
                ViewBag.ClassName = cls!.CourseName;
                return View();
            }
		}
        [HttpPost]
        public async Task<IActionResult> CreateAssignment([Bind("ClassID,AssignmentName,DueDate,PossiblePoints,Description,SubmissionType")]Assignment assignment)
        {
            var account = getAccount();

			ViewBag.isTeacher = account!.isTeacher;

            if (assignment.DueDate < DateTime.Now)
            {
				ModelState.AddModelError("DueDate", "Due date must be in the future");
			}

			if (ModelState.IsValid)
            {
                assignment.DueDate = assignment.DueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
				await _context.AddAsync(assignment);
				await _context.SaveChangesAsync();

                UpdateAssignments();

				return RedirectToAction("Index", new { id = assignment.ClassID});
			}
            else
            {
				ViewBag.ClassID = assignment.ClassID;
				var currentcls = _context.Class.Find(assignment.ClassID);
				ViewBag.ClassName = currentcls?.CourseName;
                return View("Create", assignment);
			}
		}

        [HttpGet]
        public IActionResult ToDoListClick(int assignmentID)
        {
            var account = getAccount();

            int clsID = _context.Assignments.Where(c => c.Id == assignmentID).Select(c => c.ClassID).SingleOrDefault();
            var cls = getClass(clsID); //Sets the current class

            return RedirectToAction("SubmitAssignment", new { assignmentID = assignmentID });
        }

        [HttpGet]
        public IActionResult SubmitAssignment(int assignmentID)
        {
            var account = getAccount();
            var cls = getClass();

			ViewBag.isTeacher = account!.isTeacher;

			if (cls! == null)
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
		public async Task<IActionResult> SubmitAssignment(StudentSubmission? newSubmission, IFormFile? file)
		{
            var account = getAccount();
            var cls = getClass();

            ViewBag.isTeacher = account!.isTeacher;

			if (account == null)
            {
                return NotFound();
            }

			StudentSubmission sub;
			if (ModelState.IsValid && newSubmission!.Submission != null)
            {
                newSubmission!.AccountID = account!.ID;
                if (await _context.StudentSubmissions.Where(c => c.AccountID == account!.ID && c.AssignmentID == newSubmission.AssignmentID).SingleOrDefaultAsync() == null)
                {
                    await _context.StudentSubmissions.AddAsync(newSubmission!);
                    await _context.SaveChangesAsync();
                    UpdateStudentSubmissions();
                    return RedirectToAction("Assignments");
                }
                else
                {
                    sub = await _context.StudentSubmissions.Where(c => c.AccountID == account!.ID && c.AssignmentID == newSubmission!.AssignmentID).SingleAsync();
					sub!.Submission = newSubmission.Submission;
                    _context.StudentSubmissions.Update(sub);
                    await _context.SaveChangesAsync();
                    UpdateStudentSubmissions();
                    return RedirectToAction("Assignments");
                }
            }
            else if (ModelState.IsValid && file == null)
            {
                var submissionView = new StudentSubmission
                {
                    AssignmentID = newSubmission!.AssignmentID,
                    currentAssignment = _context.Assignments.Find(newSubmission.AssignmentID)
                };
                ModelState.AddModelError("NoFile", String.Empty);
                return View(submissionView);
            }
            else if (ModelState.IsValid && file != null)
            {
				sub = newSubmission!;
				sub.AccountID = account!.ID;
				string fileName = account!.ID + "_" + newSubmission!.AssignmentID + "_" + file.FileName ;
				string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Submissions", fileName);
                using(FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    if (await _context.StudentSubmissions.Where(c => c.AccountID == account!.ID && c.AssignmentID == newSubmission!.AssignmentID).SingleOrDefaultAsync() == null)
                    {
                        await file.CopyToAsync(fs);
                        sub.Submission = fileName;
                        _context.StudentSubmissions.Add(sub);
                        await _context.SaveChangesAsync();
                        UpdateStudentSubmissions();
                        return RedirectToAction("Assignments");
                    }
                    else
                    {
                        sub = await _context.StudentSubmissions.Where(c => c.AccountID == account!.ID && c.AssignmentID == newSubmission!.AssignmentID).SingleAsync();
						await file.CopyToAsync(fs);
						sub.Submission = fileName;
						_context.StudentSubmissions.Update(sub);
                        await _context.SaveChangesAsync();
                        UpdateStudentSubmissions();
                        return RedirectToAction("Assignments");
                    }
                }
            }
            return BadRequest("Invalid File Submitted.");
        }

        [HttpGet]
        public IActionResult ViewSubmissions(int assignmentID)
        {
            var account = getAccount();
            var cls = getClass();
            ViewBag.Class = cls;

            if (account! == null || cls! == null || !account!.isTeacher)
            {
                return NotFound();
            }
            ViewBag.isTeacher = account!.isTeacher;
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

        [HttpGet]
        public IActionResult EditClass(int id)
        {
            var account = getAccount();
            var cls = getClass(id);
            ViewBag.isTeacher = account!.isTeacher;

            string str = cls.Days!;
            string day = "";
            foreach(char c in str)
            {
                if(c != ' ')
                    day += c;
                else if(c.Equals(' '))
                {
                    if (day.Equals("M"))
                    {
                        cls.monday = true;
                    }
                    else if (day.Equals("T"))
                    {
                        cls.tuesday = true;
                    }
                    else if (day.Equals("W"))
                    {
                        cls.wednesday = true;
                    }
                    else if (day.Equals("Th"))
                    {
                        cls.thursday = true;
                    }
                    else if (day.Equals("F"))
                    {
                        cls.friday = true;
                    }
                    day = "";
                }
            }
            if (account.isTeacher)
            {
                return View(cls);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditClass(Class c)
        {
            var account = getAccount();
            ViewBag.isTeacher = account.isTeacher;
            if (account.isTeacher)
            {
                setDays(ref c);
                _context.Class.Update(c);
                await _context.SaveChangesAsync();

                UpdateTeachingCourses();

                return RedirectToAction("Index");
            }
            return NotFound();

        }

        [HttpGet]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var account = getAccount();
            Class cls = _context.Class.Where(c => c.ID == id).FirstOrDefault()!;
            ViewBag.isTeacher = account.isTeacher;

            if (account.isTeacher)
            {
                _context.Class.Remove(cls);
                await _context.SaveChangesAsync();

                UpdateTeachingCourses();

                return RedirectToAction("Index");
            }

            return NotFound();

        }

        private void setDays(ref Class getDays)
        {
            getDays.Days = "";
            if (getDays.monday)
                getDays.Days += "M ";
            if (getDays.tuesday)
                getDays.Days += "T ";
            if (getDays.wednesday)
                getDays.Days += "W ";
            if (getDays.thursday)
                getDays.Days += "Th ";
            if (getDays.friday)
                getDays.Days += "F ";
        }

        private Class getClass(int? id = null)
        {
            if (UnitTestingData.isUnitTesting)
            {
                if(id != null)
                {
                    UnitTestingData.cls = _context.Class.Find(id);
                }
                return UnitTestingData.cls!;
            }
            //else
            if(id != null)
            {
                Class cCls = _context.Class.Find(id)!;
                HttpContext.Session.SetSessionValue("CurrentClass", cCls);
            }
            return HttpContext.Session.GetSessionValue<Class>("CurrentClass")!;
		}

		private Account getAccount() //Used to set the current account
		{
			if (UnitTestingData.isUnitTesting)
			{
				return UnitTestingData._account!;
			}
			return HttpContext.Session.GetSessionValue<Account>("LoggedInAccount")!;
		}

        private List<Class>? getStudentClasses()
        {
            if (!UnitTestingData.isUnitTesting)
                return HttpContext.Session.GetSessionValue<List<Class>>("RegisteredCourses")!;

            return null;
		}
        private List<Class>? getTeacherClasses()
        {
            if (!UnitTestingData.isUnitTesting)
                return HttpContext.Session.GetSessionValue<List<Class>>("TeachingCourses")!;
            return null;
        }
        private List<Assignment>? getAssignments()
        {
            if (!UnitTestingData.isUnitTesting)
                return HttpContext.Session.GetSessionValue<List<Assignment>>("Assignments");
            return null;
        }
        private List<Assignment>? getUpcomingAssignments()
        {
            if (!UnitTestingData.isUnitTesting)
                return HttpContext.Session.GetSessionValue<List<Assignment>>("Notifications");
            return null;
        }
        private List<StudentSubmission>? getStudentSubmissions()
        {
            if (!UnitTestingData.isUnitTesting)
                return HttpContext.Session.GetSessionValue<List<StudentSubmission>>("StudentSubmissions")!;
            return null;
        }

		private void UpdateTeachingCourses()
		{
            var account = getAccount();
			if (!UnitTestingData.isUnitTesting)
			{
				var teachingCourses = _context.teachingClasses.Where(tc => tc.accountID == account!.ID)
					.Join(_context.Class, tc => tc.classID, c => c.ID, (tc, c) => new Class
					{
						ID = c.ID,
						CourseNumber = $"{c.DepartmentCode} {c.CourseNumber}",
						CourseName = c.CourseName,
						Room = c.Room,
						StartTime = c.StartTime,
						EndTime = c.EndTime,
						Days = c.Days,
						color = c.color,
						hours = c.hours
					})
							.ToList();
				HttpContext.Session.SetSessionValue("TeachingCourses", teachingCourses);
			}
		}
		private void UpdateRegisteredCourses()
		{
            var account = getAccount();
			var sCourses = _context.registeredClasses.Where(rc => rc.accountID == account!.ID)
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
					   color = c.color,
					   hours = c.hours
				   }).ToList();

			foreach (var c in sCourses)
			{
				c.tName = _context.teachingClasses.Where(tc => tc.classID == c.ID)
						.Join(_context.Account, tc => tc.accountID, a => a.ID, (tc, a) => new Account
						{
							FirstName = a.FirstName,
							LastName = a.LastName
						}).Select(c => c.FirstName + " " + c.LastName).SingleOrDefault()!;
			}
			if (!UnitTestingData.isUnitTesting)
			{
				HttpContext.Session.SetSessionValue("RegisteredCourses", sCourses);
			}
		}
		private void UpdateAssignments()
        {
            var account = getAccount();

            if (!UnitTestingData.isUnitTesting)
            {
				var cls = getClass();
				List<Assignment> assignments = new List<Assignment>();
				if (!account.isTeacher)
				{
					assignments = _context.registeredClasses.Where(rc => rc.classID == cls!.ID && rc.accountID == account.ID)
						.Join(_context.Assignments, rc => rc.classID, c => c.ClassID, (rc, c) => new Assignment
						{
							Id = c.Id,
							ClassID = c.ClassID,
							AssignmentName = c.AssignmentName,
							DueDate = c.DueDate,
							PossiblePoints = c.PossiblePoints,
							Description = c.Description,
							SubmissionType = c.SubmissionType
						}).ToList();

				}
				else
				{
					assignments = _context.Assignments.Where(c => c.ClassID == cls.ID).ToList();
				}

				foreach (var a in assignments)
				{
					a.className = _context.Class.Where(c => c.ID == a.ClassID).Select(c => c.DepartmentCode + c.CourseNumber + ": " + c.CourseName).SingleOrDefault();
				}
				if (!UnitTestingData.isUnitTesting)
				{
					HttpContext.Session.SetSessionValue("Assignments", assignments);
                    if (!account.isTeacher)
                    {
                        foreach (var a in assignments)
                        {
                            a.grade = _context.StudentSubmissions.Where(c => c.AssignmentID == a.Id).Select(c => c.Grade).FirstOrDefault();
                            a.submitted = _context.StudentSubmissions.Any(ss => ss.AssignmentID == a.Id && ss.AccountID == account.ID);
                        }
                        var upcomingAssignments = assignments.Where(a => a.DueDate >= DateTime.Now && a.DueDate <= DateTime.Now.AddDays(14)).ToList();
                        HttpContext.Session.SetSessionValue("Notifications", upcomingAssignments);
                    }
				}
			}
            
        }
        private void UpdateStudentSubmissions()
        {
            var account = getAccount();
            var studentSubmissions = _context.StudentSubmissions.Where(ss => ss.AccountID == account.ID).ToList();

            if (!UnitTestingData.isUnitTesting)
                HttpContext.Session.SetSessionValue("StudentSubmissions", studentSubmissions);
        }
    }
}
