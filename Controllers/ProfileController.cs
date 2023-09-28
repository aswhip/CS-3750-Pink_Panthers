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
            ViewBag.isTeacher = _account.isTeacher;
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
                    foreach(var assignment in assignments)
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

        public IActionResult Privacy()
        {
            if(_account != null)
                return View(_account);
            return NotFound();
        }

        [HttpGet]
        public IActionResult addClass()
        {
            ViewBag.isTeacher = _account.isTeacher;
            if (_account!.isTeacher)
                return View();
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> addClass([Bind("Room,DepartmentCode,CourseNumber,CourseName,monday,tuesday,wednesday,thursday,friday,StartTime,EndTime")]Class newClass)
        {
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
                _context.Add(newClass);
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

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.isTeacher = _account.isTeacher;
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

                // Add the registeredClass to your registered classes collection
                await _context.registeredClasses.AddAsync(registeredClass);
            }
            else
            {

                // Remove the class from the student's registered classes
                var registeredClassToRemove = _context.registeredClasses.FirstOrDefault(rc => rc.accountID == accountId && rc.classID == classId);

                _context.registeredClasses.Remove(registeredClassToRemove!);
                
            }
            _context.SaveChanges();
            

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
            ViewBag.isTeacher = _account.isTeacher;
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
            ViewBag.isTeacher = _account.isTeacher;
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
            ViewBag.isTeacher = _account.isTeacher;
            return View(_account);

        }
        [HttpPost]
        public IActionResult FileUpload(IFormFile postedFile) {
            if(_account == null)
            {
                return NotFound();
            }
            string fileName = _account.ID.ToString() + "_" + _account.LastName + "pfp.jpg";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
            if(postedFile == null)
            {
                ModelState.AddModelError("NoImage", String.Empty);
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
            ViewBag.isTeacher = _account.isTeacher;
            return View(_account);
        }

        public static Account? getAccount() //Returns the account if it's not null
        {
            if (_account != null)
            {
                return _account;
            }
            return null;
        }

        public static void setAccount(ref Account account) //Used to set the current account
        {
            _account = account;
        }



        public static void logoutAccount() //Sets the current account to null
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
