using Microsoft.AspNetCore.Mvc;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using Pink_Panthers_Project.Util;
using Microsoft.EntityFrameworkCore;
using System.Data;
using OpenQA.Selenium.Internal;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OpenQA.Selenium;
using System.Diagnostics.Metrics;
using Microsoft.Build.Logging;

namespace Pink_Panthers_Project.Controllers
{
    public class ClassController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;

		public ClassController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var account = getAccount();

            if (account! == null)
            {
                return NotFound();
            }

            List<Class> tCourses = new List<Class>();
            List<Class> sCourses = new List<Class>();

            ViewBag.isTeacher = account!.isTeacher;

            UpdateAssignments();
            var assignments = getAssignments();
            var submissions = getStudentSubmissions();


            if (account!.isTeacher)
            {
                tCourses = getTeacherClasses()!;
            }
            else if (!account!.isTeacher)
            {
                sCourses = getStudentClasses()!;
            }

            var viewModel = getViewModel();

            return View(viewModel);
        }

        public IActionResult Assignments(int? id)
        {
            var account = getAccount();
            List<RGB>? colors = new List<RGB>();
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

            List<Class> sCourses = new List<Class>();

            if (!account!.isTeacher)
            {
                sCourses = getStudentClasses()!;
            }

            UpdateAssignments();
            var assignments = getAssignments();
            List<StudentSubmission>? submissions;

			List<StudentClassGrade> classPerformance = new List<StudentClassGrade>();
            int countE = 0, countDPlus = 0, countD = 0, countDMinus = 0, countCPlus = 0, countC = 0, countCMinus = 0, countBPlus = 0,
                countB = 0, countBMinus = 0, countA = 0, countAMinus = 0;

			if (account.isTeacher)
            {
                submissions = new List<StudentSubmission>();
                foreach(var a in assignments)
                {
                    var teaching = _context.StudentSubmissions.Where(tc => tc.AssignmentID == a.Id).ToList();
                    foreach(var tc in teaching)
                    {
                        tc.PossiblePoints = a.PossiblePoints;
                    }

                    submissions.AddRange(teaching);
				}
				foreach (var s in submissions)
				{
					if (!classPerformance.Any(c => c.StudentID == s.AccountID))
					{
                        if (s.Grade != null)
                        {
                            StudentClassGrade newEntry = new StudentClassGrade()
                            {
                                StudentID = s.AccountID,
                                StudentPointsEarned = s.Grade,
                                TotalPossiblePoints = 0
                            };
                            newEntry.TotalPossiblePoints += s.PossiblePoints;
                            classPerformance.Add(newEntry);
                        }
					}
					else
					{
						StudentClassGrade curStudent = classPerformance.Where(c => c.StudentID == s.AccountID).FirstOrDefault()!;
                        if (s.Grade != null)
                        {
                            curStudent.StudentPointsEarned += s.Grade;
                            curStudent.TotalPossiblePoints += s.PossiblePoints;
                        }
					}
				}

				foreach (var cp in classPerformance)
				{
                    if (cp.TotalPossiblePoints != null)
                    {
                        double percent = (double)(cp.StudentPointsEarned / (double)cp.TotalPossiblePoints);
                        percent *= 100;

                        cp.StudentGrade = getGradeLetter(percent);
                        switch (cp.StudentGrade)
                        {
                            case "A":
                                ++countA;
                                break;
                            case "A-":
                                ++countAMinus;
                                break;
                            case "B+":
                                ++countBPlus;
                                break;
                            case "B":
                                ++countB;
                                break;
                            case "B-":
                                ++countB;
                                break;
                            case "C+":
                                ++countCPlus;
                                break;
                            case "C":
                                ++countC;
                                break;
                            case "C-":
                                ++countCMinus;
                                break;
                            case "D+":
                                ++countDPlus;
                                break;
                            case "D":
                                ++countD;
                                break;
                            case "D-":
                                ++countDMinus;
                                break;
                            case "E":
                                ++countE;
                                break;
                            default:
                                break;
                        }
                    }
				}
			}
            if (!account.isTeacher)
            {
                submissions = new List<StudentSubmission>();
                foreach (var a in assignments)
                {
                    var teaching = _context.StudentSubmissions.Where(tc => tc.AssignmentID == a.Id).ToList();
                    foreach (var tc in teaching)
                    {
                        tc.PossiblePoints = a.PossiblePoints;
                    }

                    submissions.AddRange(teaching);
                }
                foreach (var s in submissions)
                {
                    if (!classPerformance.Any(c => c.StudentID == s.AccountID))
                    {
                        if (s.Grade != null)
                        {
                            StudentClassGrade newEntry = new StudentClassGrade()
                            {
                                StudentID = s.AccountID,
                                StudentPointsEarned = s.Grade,
                                TotalPossiblePoints = 0
                            };
                            newEntry.TotalPossiblePoints += s.PossiblePoints;
                            classPerformance.Add(newEntry);
                        }
                    }
                    else
                    {
                        StudentClassGrade curStudent = classPerformance.Where(c => c.StudentID == s.AccountID).FirstOrDefault()!;
                        if (s.Grade != null)
                        {
                            curStudent.StudentPointsEarned += s.Grade;
                            curStudent.TotalPossiblePoints += s.PossiblePoints;
                        }
                    }
                }

                foreach (var cp in classPerformance)
                {
                    if (cp.TotalPossiblePoints != null)
                    {
                        double percent = (double)(cp.StudentPointsEarned / (double)cp.TotalPossiblePoints);
                        percent *= 100;

                        cp.StudentGrade = getGradeLetter(percent);
                        switch (cp.StudentGrade)
                        {
                            case "A":
                                ++countA;
                                break;
                            case "A-":
                                ++countAMinus;
                                break;
                            case "B+":
                                ++countBPlus;
                                break;
                            case "B":
                                ++countB;
                                break;
                            case "B-":
                                ++countB;
                                break;
                            case "C+":
                                ++countCPlus;
                                break;
                            case "C":
                                ++countC;
                                break;
                            case "C-":
                                ++countCMinus;
                                break;
                            case "D+":
                                ++countDPlus;
                                break;
                            case "D":
                                ++countD;
                                break;
                            case "D-":
                                ++countDMinus;
                                break;
                            case "E":
                                ++countE;
                                break;
                            default:
                                break;
                        }
                    }
                }
                for (int i = 0; i < 12; ++i)
                {
                    colors.Add(new RGB(0, 0, 255, 1));
                }
                string currentStudentGrade = classPerformance.Where(c => c.StudentID == account.ID).Select(c => c.StudentGrade).SingleOrDefault();
                int index = 0;
                if (currentStudentGrade.Equals("A")) index = 0;
                else if (currentStudentGrade.Equals("A-")) index = 1;
                else if (currentStudentGrade.Equals("B+")) index = 2;
                else if (currentStudentGrade.Equals("B")) index = 3;
                else if (currentStudentGrade.Equals("B-")) index = 4;
                else if (currentStudentGrade.Equals("C+")) index = 5;
                else if (currentStudentGrade.Equals("C")) index = 6;
                else if (currentStudentGrade.Equals("C-")) index = 7;
                else if (currentStudentGrade.Equals("D+")) index = 8;
                else if (currentStudentGrade.Equals("D")) index = 9;
                else if (currentStudentGrade.Equals("D-")) index = 10;
                else if (currentStudentGrade.Equals("E")) index = 11;
                colors[index] = new RGB(0, 255, 0, 1);
            }
            else
            {
                submissions = getStudentSubmissions();
            }


            var viewModel = getViewModel();
            viewModel.countE = countE;
            viewModel.countDPlus = countDPlus;
            viewModel.countD = countD;
            viewModel.countDMinus = countDMinus;
            viewModel.countCPlus = countCPlus;
            viewModel.countC = countC;
            viewModel.countCMinus = countCMinus;
            viewModel.countBPlus = countBPlus;
            viewModel.countB = countB;
            viewModel.countBMinus = countBMinus;
            viewModel.countA = countA;
            viewModel.countAMinus = countAMinus;
            viewModel.StudentClassGrades = classPerformance;
            viewModel.GraphColor = colors;
			return View(viewModel);
		}
		[HttpGet]
		public IActionResult Performance(int assignmentID)
		{
			var account = getAccount();
			var cls = getClass();
			ViewBag.Class = cls;

			if (account! == null || cls! == null)
			{
				return NotFound();
			}
			ViewBag.isTeacher = account!.isTeacher;
			var performance = getViewModel();
			performance.StudentSubmissions = _context.StudentSubmissions.Where(c => c.AssignmentID == assignmentID).ToList();
			performance.AssignmentName = _context.Assignments.Where(c => c.Id == assignmentID).Select(c => c.AssignmentName).SingleOrDefault();
            performance.Grades = _context.StudentSubmissions.Where(c => c.AssignmentID == assignmentID && c.Grade.HasValue)
				.Select(c => c.Grade.Value)
				.ToList();
			performance.MaxGrade = _context.Assignments.Where(c => c.Id == assignmentID).Select(c => c.PossiblePoints).SingleOrDefault();


			foreach (var s in performance.StudentSubmissions)
			{
				s.studentAccount = _context.Account.Where(c => c.ID == s.AccountID).SingleOrDefault();
				s.currentAssignment = _context.Assignments.Where(c => c.Id == s.AssignmentID).SingleOrDefault();
			}
			return View(performance);
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
                Notification? notification = await _context.Notifications.Where(n => n.StudentId == sSub.AccountID
											        && n.AssignmentId == sSub.AssignmentID).SingleOrDefaultAsync();
                if (notification != null)
                {
                    notification.NotificationString = "Graded";
                    notification.IsCleared = false;

                    _context.Notifications.Update(notification);
                }
                else
                {
                    notification = new Notification();

                    notification.StudentId = sSub.AccountID;
                    notification.AssignmentId = sSub.AssignmentID;
                    notification.NotificationString = "Graded";
                    notification.IsCleared = false;

                    await _context.Notifications.AddAsync(notification);
                }

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

                var studentsInClass = await _context.registeredClasses.Where(rc => rc.classID == assignment.ClassID).ToListAsync();
                foreach(var student in studentsInClass)
                {
                    Notification notification = new Notification();
                    notification.StudentId = student.accountID;
                    notification.AssignmentId = assignment.Id;
                    notification.NotificationString = "Created";
                    notification.IsCleared = false;

                    await _context.AddAsync(notification);
                }
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
            var assignment = _context.Assignments.Find(assignmentID);
            var cls = getClass(assignment.ClassID);
           
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
                    currentAssignment = _context.Assignments.Find(assignmentID),
                    Notifications = getNotifications()
                };
                return View(submissionView);
            }
        } 
        [HttpPost]
        [DisableRequestSizeLimit]
		public async Task<IActionResult> SubmitAssignment(StudentSubmission? newSubmission, [FromForm]IFormFile? file)
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
                    currentAssignment = _context.Assignments.Find(newSubmission.AssignmentID),
                    Notifications = getNotifications()
                };
                ModelState.AddModelError("NoFile", String.Empty);
                return View(submissionView);
            }
            else if (ModelState.IsValid && file != null)
            {
				sub = newSubmission!;
				sub.AccountID = account!.ID;
				string fileName = account!.ID + "_" + newSubmission!.AssignmentID + "_" + file.FileName ;
                string path;
                if (UnitTestingData.isUnitTesting) {
                    string relativePathForTests = @"..\..\..\..\CS-3750-Pink_Panthers\wwwroot\Submissions";
                    path = Path.Combine(Directory.GetCurrentDirectory(), relativePathForTests, fileName);

                }
                else
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Submissions", fileName);
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

            if (account! == null || cls! == null)
            {
                return NotFound();
            }
            ViewBag.isTeacher = account!.isTeacher;
            var viewSubmissions = getViewModel();
            viewSubmissions.StudentSubmissions = _context.StudentSubmissions.Where(c => c.AssignmentID == assignmentID).ToList();
            viewSubmissions.AssignmentName = _context.Assignments.Where(c => c.Id == assignmentID).Select(c => c.AssignmentName).SingleOrDefault();
            viewSubmissions.Grades = _context.StudentSubmissions.Where(c => c.AssignmentID == assignmentID && c.Grade.HasValue)
                .Select(c => c.Grade.Value)
                .ToList();
            viewSubmissions.MaxGrade = _context.Assignments.Where(c => c.Id == assignmentID).Select(c => c.PossiblePoints).SingleOrDefault();

			foreach (var s in viewSubmissions.StudentSubmissions)
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
        public IActionResult EditAssignment(int AssignmentID)
        {
            var account = getAccount();
			ViewBag.isTeacher = account!.isTeacher;

			if (account.isTeacher)
            {
				var assignment = _context.Assignments.Find(AssignmentID);
                
				return View(assignment);
			}
			return NotFound();
        }

		[HttpPost]
		public async Task<IActionResult> EditAssignment(Assignment assignment)
		{
			var account = getAccount();
			ViewBag.isTeacher = account.isTeacher;
			if (!account.isTeacher)
			{
				return NotFound();
			}


			if (!ModelState.IsValid)
			{
				return View(assignment);
			}

			var existingAssignment = await _context.Assignments
				.AsNoTracking() // avoid tracking issues
				.SingleOrDefaultAsync(a => a.Id == assignment.Id);

			if (existingAssignment == null)
			{
				return NotFound();
			}
            
            if (existingAssignment.PossiblePoints != assignment.PossiblePoints)
            {
				RescaleExisitingSubmissionGrades(assignment.Id, existingAssignment.PossiblePoints, assignment.PossiblePoints);
			}

			existingAssignment.AssignmentName = assignment.AssignmentName;
			existingAssignment.Description = assignment.Description;
			existingAssignment.PossiblePoints = assignment.PossiblePoints;
			existingAssignment.DueDate = assignment.DueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
			existingAssignment.SubmissionType = assignment.SubmissionType;

			_context.Assignments.Update(existingAssignment);
			await _context.SaveChangesAsync();
			UpdateAssignments();

			return RedirectToAction("Assignments", new { id = existingAssignment.ClassID });
		}

        private void RescaleExisitingSubmissionGrades(int assignmentId, int oldPoints, int newPoints)
        {
            var submissions = _context.StudentSubmissions.Where(s => s.AssignmentID == assignmentId).ToList();
            double factor = (double)newPoints / (double)oldPoints;
            foreach (var s in submissions)
            {
				if (s.Grade != null)
                {
					s.Grade = (int)Math.Round((double)s.Grade * factor);
				}
			}
            _context.StudentSubmissions.UpdateRange(submissions);
        }

		[HttpGet]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var account = getAccount();
            Class cls = _context.Class.Where(c => c.ID == id).FirstOrDefault()!;
            Assignment assignment = _context.Assignments.Where(c => c.ClassID == cls.ID).FirstOrDefault()!;
            StudentSubmission studentSubmission = null;
            if (assignment != null)
            {
                studentSubmission = _context.StudentSubmissions.Where(c => c.AssignmentID == assignment.Id).FirstOrDefault()!;
            }
            
            ViewBag.isTeacher = account.isTeacher;

            if (account.isTeacher)
            {
                while (assignment != null)
                {
                    while (studentSubmission != null)
                    {
                        _context.StudentSubmissions.Remove(studentSubmission);
                        await _context.SaveChangesAsync();
                        studentSubmission = _context.StudentSubmissions.Where(c => c.AssignmentID == assignment.Id).FirstOrDefault()!;
                    }
                    _context.Assignments.Remove(assignment);
                    await _context.SaveChangesAsync();
                    assignment = _context.Assignments.Where(c => c.ClassID == cls.ID).FirstOrDefault()!;
                    if (assignment != null)
                    {
                        studentSubmission = _context.StudentSubmissions.Where(c => c.AssignmentID == assignment.Id).FirstOrDefault()!;
                    }
                }
                RegisteredClass registeredClass = _context.registeredClasses.Where(c => c.classID == cls.ID).FirstOrDefault()!;

                while (registeredClass != null)
                {
                    _context.registeredClasses.Remove(registeredClass);
                    await _context.SaveChangesAsync();
                    registeredClass = _context.registeredClasses.Where(c => c.classID == cls.ID).FirstOrDefault()!;
                }
                _context.Class.Remove(cls);
                await _context.SaveChangesAsync();

                UpdateTeachingCourses();

                return RedirectToAction("Index");
            }

            return NotFound();

        }

        [HttpGet]
        public async Task<IActionResult> DeleteAssignment(int assignmentId)
        {
            var account = getAccount();
            if (account == null || !account.isTeacher)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(assignmentId);
            if (assignment == null)
            {
                return NotFound();
            }

            //First delete submissions
            var submissions = await _context.StudentSubmissions.Where(s => s.AssignmentID == assignmentId).ToListAsync();
            if (submissions.Any())
            {
                _context.StudentSubmissions.RemoveRange(submissions);
                await _context.SaveChangesAsync();
            }

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            UpdateAssignments();

            return RedirectToAction("Assignments", new { id = assignment.ClassID });
        }



        private string getGradeLetter(double percent)
        {
			if (percent >= 94.00)
			{
				return "A";
			}
			else if (percent >= 90.00)
			{
				return "A-";
			}
			else if (percent >= 87.00)
			{
				return "B+";
			}
			else if (percent >= 84.00)
			{
				return "B";
			}
			else if (percent >= 80.00)
			{
				return "B-";
			}
			else if (percent >= 77.00)
			{
				return "C+";
			}
			else if (percent >= 74.00)
			{
				return "C";
			}
			else if (percent >= 70.00)
			{
				return "C-";
			}
			else if (percent >= 67.00)
			{
				return "D+";
			}
			else if (percent >= 64.00)
			{
				return "D";
			}
			else if (percent >= 60.00)
			{
				return "D-";
			}
			else
			{
				return "E";
			}
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
        private List<StudentSubmission>? getStudentSubmissions()
        {
            if (!UnitTestingData.isUnitTesting)
                return HttpContext.Session.GetSessionValue<List<StudentSubmission>>("StudentSubmissions")!;
            return null;
        }
		private List<Notification>? getNotifications()
		{
            if (!UnitTestingData.isUnitTesting)
                return HttpContext.Session.GetSessionValue<List<Notification>>("Notifications");
            return null;
		}
        private ViewModel getViewModel()
        {
            var viewModel = new ViewModel
            {
                TeachingCourses = getTeacherClasses(),
                RegisteredCourses = getStudentClasses(),
                Assignments = getAssignments(),
                StudentSubmissions = getStudentSubmissions(),
                Account = getAccount()!,
                AllAssignments = HttpContext.Session.GetSessionValue<List<Assignment>>("AllAssignments"),
                Notifications = getNotifications(),
                Class = getClass()
            };
            return viewModel;
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
                if (account != null && cls != null) // Check for null values
                {
                    
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
                        }
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
