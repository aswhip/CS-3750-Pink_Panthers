using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using System.IO;
using System.Text;
using System.Web;
using System;

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
