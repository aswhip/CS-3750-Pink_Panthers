using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using System.IO;
using System.Text;
using System.Web;
using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Collections.Immutable;
using System.Collections.Generic;
using Pink_Panthers_Project.Util;
using OpenQA.Selenium.DevTools.V115.Page;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Pink_Panthers_Project.Controllers
{
    public class ProfileController : Controller
    {
		private readonly Pink_Panthers_ProjectContext _context;

        public double AmountToPay { get; private set; }

        public ProfileController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var account = getAccount();

            ViewBag.isTeacher = account!.isTeacher;
            if (account != null) //An account must be active to view this page
            {
                var viewModel = getViewModel();
                return View(viewModel);
            }

            return NotFound();
        }


	[HttpGet]
        public IActionResult addClass()
        {
			var account = getAccount();

			ViewBag.isTeacher = account!.isTeacher;
            if (account!.isTeacher)
                return View();
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> addClass([Bind("Room,DepartmentCode,CourseNumber,CourseName,monday,tuesday,wednesday,thursday,friday,StartTime,EndTime,hours")]Class newClass)
        {
			var account = getAccount();

			if (account == null)
            {
                return NotFound();
            }
            //else
			if (account!.isTeacher)
            {
                if (!UnitTestingData.isUnitTesting && ModelState.IsValid || UnitTestingData.isUnitTesting)
                {
                    setDays(ref newClass);
                    if (String.IsNullOrEmpty(newClass.Days))
                    {
                        ModelState.AddModelError("NoDaysSelected", "");
                        return View(newClass);
                    }
                    string color = RandomHexColor();
                    newClass.color = color;
                    await _context.Class.AddAsync(newClass);
                    await _context.SaveChangesAsync();

                    newClass.ID = await _context.Class.OrderBy(c => c.ID).Select(c => c.ID).LastAsync();
                    TeachingClass teachingClass = new TeachingClass
                    {
                        accountID = account.ID,
                        classID = newClass.ID
                    };

                    await _context.teachingClasses.AddAsync(teachingClass);

                    await _context.SaveChangesAsync();

                    UpdateTeachingCourses();
                    return RedirectToAction("Index");
                }
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

        public IActionResult Chart()
        {
            var account = getAccount();

			ViewBag.isTeacher = account!.isTeacher;
            return View(getViewModel());
        }

        [HttpGet]
        public IActionResult Account([FromQuery] double amountToPay, [FromQuery] string paymentStatus)
        {
            var account = getAccount();
            ViewBag.isTeacher = account!.isTeacher;

            if (account == null)
            {
                return NotFound(); // or handle this case accordingly
            }

            if (paymentStatus == "succeeded")
            {
                // Update the account
                account.AmountToBePaid -= amountToPay;
                if (account.AmountToBePaid < 0)
                    account.AmountToBePaid = 0;

                // Save changes to the database
                _context.Account.Update(account);
                _context.SaveChanges();

                // Update the session with the latest account information
                HttpContext.Session.SetSessionValue("LoggedInAccount", account);


                // Display a success view
                return RedirectToAction("Account");
            }

            if (account != null)
            {
                var viewModel = getViewModel();
                return View(viewModel);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Account([FromQuery] double amountToPay)
        {
            var account = getAccount();

            ViewBag.isTeacher = account!.isTeacher;
            if (account != null) //An account must be active to view this page
            {
                var viewModel = getViewModel();
                account!.AmountToBePaid -= amountToPay;
                account!.AmountToBePaid = Math.Round(account!.AmountToBePaid, 2);
                if (account!.AmountToBePaid < 0)
                    account!.AmountToBePaid = 0;
                _context.Account.Update(account!);
                await _context.SaveChangesAsync();

                if(!UnitTestingData.isUnitTesting)
                    HttpContext.Session.SetSessionValue("LoggedInAccount", account);

                return View(viewModel);
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult Receipt([FromQuery] double amountToPay, [FromQuery] string paymentStatus)
        {
            var account = getAccount();
            ViewBag.isTeacher = account!.isTeacher;

            if (account == null)
            {
                return NotFound(); // or handle this case accordingly
            }

            if (paymentStatus == "succeeded")
            {
                // Update the account
                account.AmountToBePaid -= amountToPay;
                if (account.AmountToBePaid < 0)
                    account.AmountToBePaid = 0;

                // Save changes to the database
                _context.Account.Update(account);
                _context.SaveChanges();

                // Update the session with the latest account information
                HttpContext.Session.SetSessionValue("LoggedInAccount", account);

                AmountToPay = amountToPay;
                // Display a success view
                return RedirectToAction("ReceiptView");
            }

            if (account != null)
            {
                var viewModel = getViewModel();
                return View(viewModel);
            }

            return NotFound();
        }

        public IActionResult ReceiptView()
        {
            var account = getAccount();

            ViewBag.isTeacher = account!.isTeacher;
            if (account != null) //An account must be active to view this page
            {
                var pay = HttpContext.Session.GetSessionValue<decimal>("amountToPay");

                HttpContext.Session.SetSessionValue("LoggedInAccount", account);

                ViewModel viewModel = addAmountViewModel(pay);
                return View(viewModel);
            }

            return NotFound();
        }

        

        [HttpGet]
        public IActionResult Register()
        {
			var account = getAccount();

			ViewBag.isTeacher = account!.isTeacher;
            ViewBag.account = account!.ID;

            if (!account!.isTeacher && ModelState.IsValid)
            {
                var viewModel = getViewModel();
                viewModel.Classes = _context.Class.ToList();
                viewModel.RegisteredClasses = _context.registeredClasses.ToList();
                foreach(var item in viewModel.Classes)
                {
                    item.tName = _context.teachingClasses.Where(tc => tc.classID == item.ID)
                        .Join(_context.Account, tc => tc.accountID, a => a.ID, (tc, a) => new Account
                        {
                            FirstName = a.FirstName,
                            LastName = a.LastName
                        }).Select(c => c.FirstName + " " + c.LastName).SingleOrDefault()!;
                }
            return View(viewModel);
            }

            return NotFound();
        } 

        [HttpPost]
        public async Task<IActionResult> Register(int classId)
        {
			var account = getAccount();

			if (!account!.isTeacher)
            {
                int accountId = account!.ID;

                var existingRegistration = _context.registeredClasses.FirstOrDefault(rc => rc.accountID == accountId && rc.classID == classId);

                if (existingRegistration == null)
                {


                    // Add the class to the student's registered classes
                    var registeredClass = new RegisteredClass
                    {
                        accountID = account!.ID,
                        classID = classId
                    };

                    int hours = _context.Class.Where(c => c.ID == registeredClass.classID).Select(c => c.hours).SingleOrDefault();
                    
                    account!.AmountToBePaid += (100 * hours);
                    account!.AmountToBePaid = Math.Round(account!.AmountToBePaid, 2);
                    if (account.AmountToBePaid <= 0)
                    {
                        account.AmountToBePaid = 0;
                    }
                    _context.Account.Update(account);
                    // Add the registeredClass to your registered classes collection
                    await _context.registeredClasses.AddAsync(registeredClass);
                }
                else
                {

                    // Remove the class from the student's registered classes
                    var registeredClassToRemove = _context.registeredClasses.FirstOrDefault(rc => rc.accountID == accountId && rc.classID == classId);

                    int hours = _context.Class.Where(c => c.ID == registeredClassToRemove!.classID).Select(c => c.hours).SingleOrDefault();
                    
                    account!.AmountToBePaid -= (100 * hours);
                    account!.AmountToBePaid = Math.Round(account!.AmountToBePaid, 2);
                    if (account.AmountToBePaid <= 0)
                    {
                        account.AmountToBePaid = 0;
                    }
                    _context.Account.Update(account);

					_context.registeredClasses.Remove(registeredClassToRemove!);

                }
                await _context.SaveChangesAsync();
            }
            UpdateRegisteredCourses();
            UpdateAccount(account.ID);
            // Redirect back to the class list page
            return RedirectToAction("Index");
        }


        /// <summary>
        /// GET
        /// Returns page with details of the logged in account
        /// </summary>
        /// <returns></returns>
        public IActionResult Details()
        {
			var account = getAccount();

			ViewBag.isTeacher = account!.isTeacher;
            if (account != null)
            {
                var viewModel = getViewModel();
                return View(viewModel);
            }
            return NotFound();
        }

        /// <summary>
        /// GET
        /// Returns page with now editable fields
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit()
        {
			var account = getAccount();
            account.Notifications = getNotifications();

            ViewBag.isTeacher = account!.isTeacher;
            if (account != null)
                return View(account);
            return NotFound();
        }

        /// <summary>
        /// POST
        /// Updates account details
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Account account)
        {

            if(!ModelState.IsValid) //Check if fields are vaild
            {
                return View(account);
            }
            Account? a = await _context.Account.Where(c => c.ID == account.ID).SingleOrDefaultAsync();
            if (a != null)
            {
                a.FirstName = account.FirstName;
                a.LastName = account.LastName;
                a.AddressLine1 = account.AddressLine1;
                a.AddressLine2 = account.AddressLine2;
                a.City = account.City;
                a.State = account.State;
                a.ZipCode = account.ZipCode;
                a.ProfileLink1 = account.ProfileLink1;
                a.ProfileLink2 = account.ProfileLink2;
                a.ProfileLink3 = account.ProfileLink3;
                _context.Account.Update(a);
                await _context.SaveChangesAsync();
            }
            else
                return NotFound();

            UpdateAccount(account.ID);
            return RedirectToAction("Details");
        }
        [HttpGet]
        public IActionResult FileUpload()
        {
			var account = getAccount();

			ViewBag.isTeacher = account!.isTeacher;
            return View(getViewModel());

        }
        [HttpPost]
        [DisableRequestSizeLimit]
        public IActionResult FileUpload(IFormFile postedFile) 
        {
			var account = getAccount();

			if (account == null)
            {
                return NotFound();
            }
            string fileName = account!.ID.ToString() + "_" + account!.LastName + "pfp.jpg";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
            if (postedFile == null)
            {
                ModelState.AddModelError("NoImage", String.Empty);
                ViewBag.isTeacher = account!.isTeacher;
                return View();
            }
            if(postedFile != null)
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    postedFile.CopyTo(fs);
                }
                return RedirectToAction("Details");
            }
            return BadRequest("Image not Found");
        }
        public IActionResult Calendar()
        {
            var account = getAccount();

			ViewBag.isTeacher = account!.isTeacher;
            if (account != null) //An account must be active to view this page
            {
                var viewModel = getViewModel();
                return View(viewModel);
            }
            return NotFound();
        }

        public IActionResult ClearNotification(int notificationID, string returnUrl)
        {
            _context.Notifications.Where(n => n.Id == notificationID).ExecuteUpdate(n => n.SetProperty(c => c.IsCleared, true));
            UpdateNotifications(getAccount());

            return RedirectPermanent(returnUrl);
        }

        
        /// <summary>
        /// Getters and Setters for the viewModels and account, based on session
        /// </summary>
        /// <returns></returns>
        private Account getAccount() //Used to set the current account
        {
			if (UnitTestingData.isUnitTesting)
			{
                return UnitTestingData._account!;
			}
			return HttpContext.Session.GetSessionValue<Account>("LoggedInAccount")!;
			
		}
        public void logoutAccount() //Sets the current account to null
        {
			if (UnitTestingData.isUnitTesting)
			{
				UnitTestingData._account = null;
			}
			else
			{
				HttpContext.Session.Remove("LoggedInAccount");
                HttpContext.Session.Clear();
			}
		}

        private ViewModel getViewModel()
        {
            ViewModel viewModel;
            if (UnitTestingData.isUnitTesting)
            {
                viewModel = new ViewModel
                {
                    Account = getAccount()
                };
            }
            else
            {
                viewModel = new ViewModel
                {
                    TeachingCourses = getTeacherClasses(),
                    RegisteredCourses = getStudentClasses(),
                    Assignments = HttpContext.Session.GetSessionValue<List<Assignment>>("Assignments"),
                    StudentSubmissions = HttpContext.Session.GetSessionValue<List<StudentSubmission>>("StudentSubmissions"),
                    AllAssignments = HttpContext.Session.GetSessionValue<List<Assignment>>("AllAssignments"),
                    Account = getAccount(),
                    Notifications = getNotifications()
                };
            }
            return viewModel;
		}

        private ViewModel addAmountViewModel(decimal amountToPay)
        {
            ViewModel viewModel;
            if (UnitTestingData.isUnitTesting)
            {
                viewModel = new ViewModel
                {
                    Account = getAccount()
                };
            }
            else
            {
                viewModel = new ViewModel
                {
                    TeachingCourses = getTeacherClasses(),
                    RegisteredCourses = getStudentClasses(),
                    Assignments = HttpContext.Session.GetSessionValue<List<Assignment>>("Assignments"),
                    StudentSubmissions = HttpContext.Session.GetSessionValue<List<StudentSubmission>>("StudentSubmissions"),
                    AllAssignments = HttpContext.Session.GetSessionValue<List<Assignment>>("AllAssignments"),
                    Account = getAccount(),
                    Notifications = getNotifications(),
                    AmountToPay = amountToPay
                };
            }
            return viewModel;
        }

		private List<Class>? getTeacherClasses()
		{
			if (!UnitTestingData.isUnitTesting)
				return HttpContext.Session.GetSessionValue<List<Class>>("TeachingCourses")!;
			return null;
		}
        private List<Notification>? getNotifications()
        {
            if (!UnitTestingData.isUnitTesting)
                return HttpContext.Session.GetSessionValue<List<Notification>>("Notifications");
            return null;
        }
		private List<Class>? getStudentClasses()
		{
			if (!UnitTestingData.isUnitTesting)
				return HttpContext.Session.GetSessionValue<List<Class>>("RegisteredCourses")!;

			return null;
		}
        private void UpdateAccount(int id)
        {
            if (!UnitTestingData.isUnitTesting)
            {
                var account = _context.Account.Where(a => a.ID == id).FirstOrDefault();
                HttpContext.Session.SetSessionValue("LoggedInAccount", account);
            }
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
            var assignments = _context.registeredClasses.Where(rc => rc.accountID == account!.ID)
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

            foreach (var a in assignments)
            {
                a.className = _context.Class.Where(c => c.ID == a.ClassID).Select(c => c.DepartmentCode + c.CourseNumber + ": " + c.CourseName).SingleOrDefault();
                a.grade = _context.StudentSubmissions.Where(c => c.AssignmentID == a.Id).Select(c => c.Grade).FirstOrDefault();
                a.submitted = _context.StudentSubmissions.Any(ss => ss.AssignmentID == a.Id && ss.AccountID == account.ID);
            }
            if (!UnitTestingData.isUnitTesting)
            {
                HttpContext.Session.SetSessionValue("Assignments", assignments);
            }
        }

        private void UpdateNotifications(Account account)
        {
            var notifications = _context.Notifications.Where(n => n.StudentId == account.ID && n.IsCleared == false).ToList();
            foreach (var notification in notifications)
            {
                var assignment = _context.Assignments.Where(a => a.Id == notification.AssignmentId).FirstOrDefault();
                notification.AssignmentName = assignment.AssignmentName;
                notification.DueDate = assignment.DueDate;
                notification.ClassName = _context.Class.Where(c => c.ID == assignment.ClassID).Select(c => c.DepartmentCode.ToString() + c.CourseNumber.ToString() + ": " + c.CourseName).SingleOrDefault();
            }
            if (!UnitTestingData.isUnitTesting)
                HttpContext.Session.SetSessionValue("Notifications", notifications);
        }


        /// <summary>
        /// Other functions
        /// </summary>
        /// <returns></returns>
        string RandomHexColor()
        {
            Random random = new Random();
            string hexColor;

            double brightness;

            // Generate a random color
            int red = random.Next(256);
            int green = random.Next(256);
            int blue = random.Next(256);

            // Calculate brightness
            brightness = 0.299 * red + 0.587 * green + 0.114 * blue;

            // Convert RGB to hex
            hexColor = $"#{red:X2}{green:X2}{blue:X2}";

            if (brightness > 128)
            {
                hexColor += ";color:#000";
            }
            // Check if brightness is above the threshold (e.g., 128)


            return hexColor;
        }
    }
}
