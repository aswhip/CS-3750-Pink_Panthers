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
            if(_account != null) //An account must be active to view this page
            {
                getDays(1);
                getDays(2);
                getDays(3);
                return View(_account);
            }
            return NotFound();
        }

        public IActionResult Privacy()
        {
            if(_account != null)
                return View();
            return NotFound();
        }

        [HttpGet]
        public IActionResult addClass()
        {
            if (_account!.isTeacher)
                return View();
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> addClass([Bind("Room,DepartmentCode,CourseNumber,CourseName,StartTime,EndTime")]Class newClass, [Bind("monday,tuesday,wednesday,thursday,friday")]Class getDays)
        {
            if (_account!.isTeacher && ModelState.IsValid)
            {
                int days = setDays(ref getDays);
                if(days == 0)
                {
                    ModelState.AddModelError("NoDaysSelected", "");
                    return View(newClass);
                }
                newClass.Days = days;
                newClass.accountID = _account.ID;
                _context.Add(newClass);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        private int setDays(ref Class getDays)
        {
            int days = 0;
            if (getDays.monday)
                days += 1;
            if (getDays.tuesday)
                days += 2;
            if(getDays.wednesday)
                days += 4;
            if (getDays.thursday)
                days += 8;
            if (getDays.friday)
                days += 16;
            return days;
        }

        /* Use this class as a template for when you are getting the days from the database
         * This uses bit magic using Bit Shifting and Bit AND to check each day
         * If you need help understanding it let Nathan know
         */
        private void getDays(int id) //Just using this for test cases with bit magic
        {
            Class? @class = _context.Class.FirstOrDefault(m => m.ID == id); //ID 1 only has Thursday, ID 2 has Monday,Wednesday,Friday, ID 3 has all
            int days = @class!.Days;
            int curDay = 0;
            int temp = 1;
            for(int i = 0; i < 5; ++i)
            {
                curDay = temp & (days >> i); //Funny bit magic hehe
                if (curDay == 1)
                {
                    switch (i)
                    {
                        case 0:
                            Console.WriteLine("Monday");
                            break;
                        case 1:
                            Console.WriteLine("Tuesday");
                            break;
                        case 2: 
                            Console.WriteLine("Wednesday");
                            break;
                        case 3:
                            Console.WriteLine("Thursday");
                            break;
                        case 4:
                            Console.WriteLine("Friday");
                            break;

                    }
                }
            }
        }


        /// <summary>
        /// GET
        /// Returns page with details of the logged in account
        /// </summary>
        /// <returns></returns>
        public IActionResult Details()
        {
            if(_account != null)
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
            if(_account != null)
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
            return View();

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
                return View();
            }
            return BadRequest("Image not Found");
        }
        public IActionResult Calendar()
        {
            return View();
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
    }
}
