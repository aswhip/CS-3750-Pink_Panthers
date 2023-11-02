using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using Pink_Panthers_Project.Util;

namespace Pink_Panthers_Project.Controllers
{
    public class AccountsController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;
        public AccountsController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Email,Password")] Account account)
        {
            Account? loginAccount = null;
            if(String.IsNullOrEmpty(account.Email) || String.IsNullOrEmpty(account.Password))
            {
                ModelState.AddModelError("MissingInfo", "");
                return View();
            }
            if (!String.IsNullOrEmpty(account.Email) && !String.IsNullOrEmpty(account.Password)) //Email and Password must be entered
            {
                loginAccount = _context.Account.FirstOrDefault(m => m.Email == account.Email); //Tries to find an account with the passed in email
            }
            if (loginAccount == null && !String.IsNullOrEmpty(account.Email)) //Comparing against the account.email so we don't get the error message on load
            {
                ModelState.AddModelError("InvalidEmail", "");
                return View();
            }
            else if (loginAccount != null)
            {
                if (!validatePassword(ref account, ref loginAccount)) //Checks the entered password
                {
                    ModelState.AddModelError("IncorrectPass", "");
                    return View();
                }
                else
                {
					if (UnitTestingData.isUnitTesting)
					{
						UnitTestingData._account = loginAccount;
					}
					else
					{
                        if (loginAccount.isTeacher)
                        {
                            UpdateTeachingCourses(loginAccount);
                        }
                        else
                        {
                            UpdateRegisteredCourses(loginAccount);
                            UpdateAssignments(loginAccount);
                            UpdateStudentSubmissions(loginAccount);
							UpdateNotifications(loginAccount);
						}
                        HttpContext.Session.SetSessionValue("LoggedInAccount", loginAccount);
					}
					return RedirectToAction(nameof(ProfileController.Index), "Profile"); //If email and password match, take us to the logged in page
                }
            }
            return View();

        }


        // GET: Accounts/Create
        public IActionResult Create() //Initial Load of the page
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Password,ConfirmPassword,BirthDate,isTeacher")] Account account)
        {
            if (ModelState.IsValid) //If all binded properties have a value, the state is valid
            {
                if (isOldEnough(ref account) && !emailExists(account.Email!)) //Checks the age and email for validity.
                {
                    if (account.Password!.Equals(account.ConfirmPassword!)) //account.Password and account.ConfirmPassword cannot be null here
                    {
                        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); //Generates a new hashing algorithm salt per account, so no two accounts share the same one
                        account.Salt = Convert.ToBase64String(salt); //Convert it to a string to store in the database
                        hashPassword(ref account); //Reference so we aren't creating a new temporary account
                        await _context.AddAsync(account); //Adds the account to the database
                        await _context.SaveChangesAsync();

						if (UnitTestingData.isUnitTesting)
						{
							UnitTestingData._account = account;
						}
						else
						{
							HttpContext.Session.SetSessionValue("LoggedInAccount", account);
						}
                        UpdateRegisteredCourses(account);
                        UpdateTeachingCourses(account);
                        UpdateAssignments(account);
                        UpdateStudentSubmissions(account);
                        UpdateNotifications(account);
						return RedirectToAction(nameof(ProfileController.Index), "Profile"); //Redirect to the Logged-in page. Name can be changed if need be
                    }
                    else
                    {
                        ModelState.AddModelError("PasswordsDontMatch", ""); //Error for when passwords don't match
                    }
                }
                else
                {
                    if(!isOldEnough(ref account))
                        ModelState.AddModelError("NotOldEnough", ""); //Error for when the user isn't old enough
                    if(emailExists(account.Email!))
                        ModelState.AddModelError("AccountExists", ""); //Error for when an account with the email exists already
                }
            }
            return View(account);
        }

        /// <summary>
        /// Private functions for setting and testing parameters
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool emailExists(string email) //If an account with the passed in email exists, return true, else return false
        {
            return (_context.Account?.Any(a => a.Email == email)).GetValueOrDefault();
        }

        //Not the most secure approach, but for what we need having this be its own function works great
        private void hashPassword(ref Account account)
        {
            string hashedPassword = getHashedPassword(account.Password!, Encoding.ASCII.GetBytes(account.Salt!));
            account.Password = hashedPassword; //Makes the account password the hashed password for storing in the database
            account.ConfirmPassword = hashedPassword;
        }

		//Validates the password by hashing this password and comparing the hashed passwords
		private bool validatePassword(ref Account account, ref Account loginAccount) 
        {
            string hashedPassword = getHashedPassword(account.Password!, Encoding.ASCII.GetBytes(loginAccount.Salt!));

            if (hashedPassword.Equals(loginAccount.Password))
            {
                return true;
            }
            return false;
        }

        //Has to be its own function or the hashing can return different values even using the same hashing algorithm salt
        private string getHashedPassword(string password, byte[] salt)
        {
            string hashedPassword = "";
            hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2( //Microsoft's hashing algorithm
                password: password!, //Password can't be null here
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256, //SHA256 algorithm
                iterationCount: 300000, //300,000 iterations is the recommended amount right now
                numBytesRequested: 256 / 8));

            return hashedPassword;
        }

        private bool isOldEnough(ref Account account)
        {
			//Adds 18 years to the user's entered birthdate. If the date is still before or equal to today's date, they are 18 years old.
			return (account.BirthDate.AddYears(18) <= DateTime.Now);  
        }

		private void UpdateTeachingCourses(Account account)
		{
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

        private void UpdateRegisteredCourses(Account account)
        {
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

            foreach(var c in sCourses)
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
        private void UpdateAssignments(Account account)
        {
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

            foreach(var a in assignments)
            {
                a.className = _context.Class.Where(c => c.ID == a.ClassID).Select(c => c.DepartmentCode + c.CourseNumber + ": " + c.CourseName).SingleOrDefault();
                a.grade = _context.StudentSubmissions.Where(c => c.AssignmentID == a.Id).Select(c => c.Grade).FirstOrDefault();
                a.submitted = _context.StudentSubmissions.Any(ss => ss.AssignmentID == a.Id && ss.AccountID == account.ID);
            }
            if (!UnitTestingData.isUnitTesting)
            {
                HttpContext.Session.SetSessionValue("Assignments", assignments);
                HttpContext.Session.SetSessionValue("AllAssignments", assignments);
            }
        }
        private void UpdateStudentSubmissions(Account account)
        {
            var studentSubmissions = _context.StudentSubmissions.Where(ss => ss.AccountID == account.ID).ToList();

            if (!UnitTestingData.isUnitTesting)
                HttpContext.Session.SetSessionValue("StudentSubmissions", studentSubmissions);
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
	}
}
