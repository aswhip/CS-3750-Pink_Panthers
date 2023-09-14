using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;

namespace Pink_Panthers_Project.Controllers
{
    public class AccountsController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;

        public AccountsController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }

        public IActionResult Login([Bind("Email,Password")] Account account)
        {
            Account? loginAccount = null;
            if (!String.IsNullOrEmpty(account.Email) && !String.IsNullOrEmpty(account.Password))
            {
                loginAccount = _context.Account.FirstOrDefault(m => m.Email == account.Email);
            }
            if (loginAccount == null && !String.IsNullOrEmpty(account.Email))
            {
                ModelState.AddModelError("InvalidEmail", "");
                return View();
            }
            else if (loginAccount != null)
            {
                if (!validatePassword(ref account, ref loginAccount))
                {
                    ModelState.AddModelError("IncorrectPass", "");
                    return View();
                }
                else
                {
                    return RedirectToAction(nameof(ProfileController.Index), "Profile", loginAccount);
                }
            }
            return View();
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Password,ConfirmPassword,BirthDate,accountType")] Account account)
        {
            //bool accountType = Boolean.Parse(account.accountType);
            if (ModelState.IsValid)
            {
                if (isOldEnough(ref account))
                {
                    if (!emailExists(account.Email!))
                    {
                        if (account.Password.Equals(account.ConfirmPassword))
                        {
                            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
                            account.Salt = Convert.ToBase64String(salt);
                            hashPassword(ref account);
                            _context.Add(account);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(ProfileController.Index), "Profile", account);
                        }
                        else
                        {
                            ModelState.AddModelError("PasswordsDontMatch", "");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("AccountExists", "");
                    }
                }
                else
                {
                    ModelState.AddModelError("NotOldEnough", "");
                }
            }
            return View(account);
        }

        private bool AccountExists(int id)
        {
            return (_context.Account?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        private bool emailExists(string email)
        {
            return (_context.Account?.Any(a => a.Email == email)).GetValueOrDefault();
        }

        private void hashPassword(ref Account account)
        {
            string hashedPassword = getHashedPassword(account.Password!, Encoding.ASCII.GetBytes(account.Salt!));
            account.Password = hashedPassword;
            account.ConfirmPassword = hashedPassword;
        }

        private bool validatePassword(ref Account account, ref Account loginAccount)
        {
            string hashedPassword = getHashedPassword(account.Password!, Encoding.ASCII.GetBytes(loginAccount.Salt!));

            if (hashedPassword.Equals(loginAccount.Password))
            {
                return true;
            }
            return false;
        }

        private string getHashedPassword(string password, byte[] salt)
        {
            string hashedPassword = "";
            hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 300000,
                numBytesRequested: 256 / 8));

            return hashedPassword;
        }

        private bool isOldEnough(ref Account account)
        {
            //int age = DateTime.Now.Year - date.Year;
            return (account.BirthDate.AddYears(18) <= DateTime.Now);
        }
    }
}
