﻿using Microsoft.AspNetCore.Mvc;
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
using Pink_Panthers_Project.Migrations;
using System.Collections.Immutable;
using Pink_Panthers_Project.Util;

namespace Pink_Panthers_Project.Controllers
{
    public class ProfileController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;
        public ProfileController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }
        private static Account? _account;

        public IActionResult Index()
        {
            setAccount();
            ClassController.resetClass();
            ViewBag.isTeacher = _account!.isTeacher;
            if(_account != null) //An account must be active to view this page
            {
                var teachingCourses = new List<Class>();//list of classes an instructor is teaching
                var registeredCourses = new List<Class>();//list of classes a student is taking
                var assignments = new List<Assignment>();

                if (_account.isTeacher)//check if the user is a teacher
                {
                    teachingCourses = _context.Class //populate the fields
                        .Where(c => c.accountID == _account.ID)
                        .Select(c => new Class
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
                }
                else
                {
                    registeredCourses = _context.registeredClasses
                .Where(rc => rc.accountID == _account.ID)
                .Join(_context.Class, rc => rc.classID, c => c.ID, (rc, c) => new Class
                {
                    ID = c.ID,
                    CourseNumber = $"{c.DepartmentCode} {c.CourseNumber}",
                    CourseName = c.CourseName,
                    Room = c.Room,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    Days = c.Days,
                    tName = _context.Account.Where(t => t.ID == c.accountID).Select(n => n.FirstName + " " + n.LastName).SingleOrDefault(),
                    color = c.color,
                    hours = c.hours
                })
                .ToList();
                    assignments = _context.registeredClasses.Where(rc => rc.accountID == _account.ID)
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

                foreach(var a in assignments)
                {
                    a.className = _context.Class.Where(c => c.ID == a.ClassID).Select(c => c.DepartmentCode + c.CourseNumber + ": " + c.CourseName).SingleOrDefault();
                }

                var viewModel = new CourseView
                {
                    TeachingCourses = teachingCourses,
                    RegisteredCourses = registeredCourses,
                    Assignments = assignments,
                    Account = _account
                };
                return View(viewModel);
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult addClass()
        {
			ViewBag.isTeacher = _account!.isTeacher;
            if (_account!.isTeacher)
                return View();
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> addClass([Bind("Room,DepartmentCode,CourseNumber,CourseName,monday,tuesday,wednesday,thursday,friday,StartTime,EndTime,hours")]Class newClass)
        {
            if(_account == null)
            {
                return NotFound();
            }
            //else
			if (_account!.isTeacher && ModelState.IsValid)
            {
                setDays(ref newClass);
                if (String.IsNullOrEmpty(newClass.Days))
                {
                    ModelState.AddModelError("NoDaysSelected", "");
                    return View(newClass);
                }
                newClass.accountID = _account.ID;
                string color = RandomHexColor();
                newClass.color = color;
                await _context.AddAsync(newClass);
                await _context.SaveChangesAsync();
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

        public IActionResult Chart()
        {
			ViewBag.isTeacher = _account!.isTeacher;
            return View();
        }

        [HttpGet]
        public IActionResult Account()
        {
			ViewBag.isTeacher = _account!.isTeacher;
            if (_account != null) //An account must be active to view this page
            {
                var teachingCourses = new List<Class>();//list of classes an instructor is teaching
                var registeredCourses = new List<Class>();//list of classes a student is taking
                var assignments = new List<Assignment>();

                registeredCourses = _context.registeredClasses
                .Where(rc => rc.accountID == _account.ID)
                .Join(_context.Class, rc => rc.classID, c => c.ID, (rc, c) => new Class
                {
                    CourseNumber = $"{c.DepartmentCode} {c.CourseNumber}",
                    CourseName = c.CourseName,
                    Room = c.Room,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    Days = c.Days,
                    tName = _context.Account.Where(t => t.ID == c.accountID).Select(n => n.FirstName + " " + n.LastName).SingleOrDefault(),
                    color = c.color,
                    hours = c.hours
                })
                .ToList();

                assignments = _context.Assignments.OrderBy(c => c.Id).ToList();
                foreach (var assignment in assignments)
                {
                    assignment.className = _context.Class.Where(c => c.ID == assignment.ClassID).Select(c => c.DepartmentCode + c.CourseNumber + ": " + c.CourseName).SingleOrDefault();
                }
                var viewModel = new CourseView
                {
                    TeachingCourses = teachingCourses,
                    RegisteredCourses = registeredCourses,
                    Assignments = assignments,
                    Account = _account
                };
                return View(viewModel);
            }


            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Account(double amountToPay) //Updates the amount needing to be paid
        {
			ViewBag.isTeacher = _account!.isTeacher;
            if (_account != null) //An account must be active to view this page
            {
                var teachingCourses = new List<Class>();//list of classes an instructor is teaching
                var registeredCourses = new List<Class>();//list of classes a student is taking
                var assignments = new List<Assignment>();

                registeredCourses = _context.registeredClasses
                .Where(rc => rc.accountID == _account.ID)
                .Join(_context.Class, rc => rc.classID, c => c.ID, (rc, c) => new Class
                {
                    CourseNumber = $"{c.DepartmentCode} {c.CourseNumber}",
                    CourseName = c.CourseName,
                    Room = c.Room,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    Days = c.Days,
                    tName = _context.Account.Where(t => t.ID == c.accountID).Select(n => n.FirstName + " " + n.LastName).SingleOrDefault(),
                    color = c.color,
                    hours = c.hours
                })
                .ToList();

                assignments = _context.Assignments.OrderBy(c => c.Id).ToList();
                foreach (var assignment in assignments)
                {
                    assignment.className = _context.Class.Where(c => c.ID == assignment.ClassID).Select(c => c.DepartmentCode + c.CourseNumber + ": " + c.CourseName).SingleOrDefault();
                }
                var viewModel = new CourseView
                {
                    TeachingCourses = teachingCourses,
                    RegisteredCourses = registeredCourses,
                    Assignments = assignments,
                    Account = _account
                };

                _account!.AmountToBePaid -= amountToPay;
                _account!.AmountToBePaid = Math.Round(_account!.AmountToBePaid, 2);
                if (_account!.AmountToBePaid < 0)
                    _account!.AmountToBePaid = 0;
                _context.Account.Update(_account);
                await _context.SaveChangesAsync();
                return View(viewModel);
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult Register()
        {
			ViewBag.isTeacher = _account!.isTeacher;
            ViewBag.account = _account!.ID;

            if (!_account!.isTeacher && ModelState.IsValid)
            {
                var viewModel = new RegisterView
                {
                    Classes = _context.Class.ToList(),
                    RegisteredClasses = _context.registeredClasses.ToList(),
                };
                foreach(var item in viewModel.Classes)
                {
                    item.tName = _context.Account.Where(t => t.ID == item.accountID).Select(n => n.FirstName + " " + n.LastName).SingleOrDefault();
                }
            return View(viewModel);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Register(int classId)
        {
			if (!_account!.isTeacher)
            {
                int accountId = _account!.ID;

                var existingRegistration = _context.registeredClasses.FirstOrDefault(rc => rc.accountID == accountId && rc.classID == classId);

                if (existingRegistration == null)
                {


                    // Add the class to the student's registered classes
                    var registeredClass = new RegisteredClass
                    {
                        accountID = _account.ID,
                        classID = classId
                    };

                    int hours = _context.Class.Where(c => c.ID == registeredClass.classID).Select(c => c.hours).SingleOrDefault();
                    _account.AmountToBePaid += (100 * hours);
                    _account!.AmountToBePaid = Math.Round(_account!.AmountToBePaid, 2);

                    // Add the registeredClass to your registered classes collection
                    await _context.registeredClasses.AddAsync(registeredClass);
                }
                else
                {

                    // Remove the class from the student's registered classes
                    var registeredClassToRemove = _context.registeredClasses.FirstOrDefault(rc => rc.accountID == accountId && rc.classID == classId);

                    int hours = _context.Class.Where(c => c.ID == registeredClassToRemove!.classID).Select(c => c.hours).SingleOrDefault();
                    _account.AmountToBePaid -= (100 * hours);
                    _account!.AmountToBePaid = Math.Round(_account!.AmountToBePaid, 2);

                    _context.registeredClasses.Remove(registeredClassToRemove!);

                }
                await _context.SaveChangesAsync();
            }
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
			ViewBag.isTeacher = _account!.isTeacher;
            if (_account != null)
                return View(_account);
            return NotFound();
        }

        /// <summary>
        /// GET
        /// Returns page with now editable fields
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit()
        {
			ViewBag.isTeacher = _account!.isTeacher;
            if (_account != null)
                return View(_account);
            return NotFound();
        }

        /// <summary>
        /// POST
        /// Updates account details
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit(Account account)
        {
			var Account = _account; //Get account

            if(!ModelState.IsValid) //Check if fields are vaild
            {
                return View(account);
            }

            if (Account != null) //Update account information
            {
                Account.FirstName = account.FirstName;
                Account.LastName = account.LastName;
                Account.AddressLine1 = account.AddressLine1;
                Account.AddressLine2 = account.AddressLine2;
                Account.City = account.City;
                Account.State = account.State;
                Account.ZipCode = account.ZipCode;
                Account.ProfileLink1 = account.ProfileLink1;
                Account.ProfileLink2 = account.ProfileLink2;
                Account.ProfileLink3 = account.ProfileLink3;

                //Save to database
                _context.Entry(Account).State = EntityState.Modified;
                _context.SaveChanges();
            }
            else
            {
                return NotFound();
            }


            return RedirectToAction("Details");
        }
        [HttpGet]
        public IActionResult FileUpload(){
			ViewBag.isTeacher = _account!.isTeacher;
            return View(_account);

        }
        [HttpPost]
        public IActionResult FileUpload(IFormFile postedFile) {
			if (_account == null)
            {
                return NotFound();
            }
            string fileName = _account.ID.ToString() + "_" + _account.LastName + "pfp.jpg";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
            if (postedFile == null)
            {
                ModelState.AddModelError("NoImage", String.Empty);
                ViewBag.isTeacher = _account.isTeacher;
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
			ViewBag.isTeacher = _account!.isTeacher;
            if (_account != null) //An account must be active to view this page
            {
                var teachingCourses = new List<Class>();//list of classes an instructor is teaching
                var registeredCourses = new List<Class>();//list of classes a student is taking
                var assignments = new List<Assignment>();

                if (_account.isTeacher)//check if the user is a teacher
                {
                    teachingCourses = _context.Class //populate the fields
                        .Where(c => c.accountID == _account.ID)
                        .Select(c => new Class
                        {
                            CourseNumber = $"{c.DepartmentCode} {c.CourseNumber}",
                            CourseName = c.CourseName,
                            Room = c.Room,
                            StartTime = c.StartTime,
                            EndTime = c.EndTime,
                            Days = c.Days,
                            color = c.color
                        })
                        .ToList();
                }
                else
                {
                    registeredCourses = _context.registeredClasses
                .Where(rc => rc.accountID == _account.ID)
                .Join(_context.Class, rc => rc.classID, c => c.ID, (rc, c) => new Class
                {
                    CourseNumber = $"{c.DepartmentCode} {c.CourseNumber}",
                    CourseName = c.CourseName,
                    Room = c.Room,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    Days = c.Days,
                    tName = _context.Account.Where(t => t.ID == c.accountID).Select(n => n.FirstName + " " + n.LastName).SingleOrDefault(),
                    color = c.color
                })
                .ToList();

                    assignments = _context.Assignments.OrderBy(c => c.Id).ToList();
                    foreach (var assignment in assignments)
                    {
                        assignment.className = _context.Class.Where(c => c.ID == assignment.ClassID).Select(c => c.DepartmentCode + c.CourseNumber + ": " + c.CourseName).SingleOrDefault();
                    }
                }

                var viewModel = new CourseView
                {
                    TeachingCourses = teachingCourses,
                    RegisteredCourses = registeredCourses,
                    Assignments = assignments,
                    Account = _account
                };
                return View(viewModel);
            }
            return NotFound();
        }

        public void setAccount(Account? account = null, bool isUnitTest = false) //Used to set the current account
        {
            if(isUnitTest)
                _account = account;
            else
            {
                _account = HttpContext.Session.GetSessionValue<Account>("LoggedInAccount");
            }
		}
        public void logoutAccount() //Sets the current account to null
        {
            _account = null;
        }

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
