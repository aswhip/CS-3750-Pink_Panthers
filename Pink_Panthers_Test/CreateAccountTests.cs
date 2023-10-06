using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Controllers;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pink_Panthers_Test
{
    [TestClass]
    public class CreateAccountTests : Tests //Derived from Tests, can access protected member _context
    {
        [TestMethod]
        public async Task CanCreateStudentAccount()
        {
			UnitTestingData.isUnitTesting = true;

			AccountsController accountsController = new AccountsController(_context);
            Account newAccount = new Account
            {
                FirstName = "Unit",
                LastName = "Test Student",
                Email = "unittest@gmail.com",
                Password = "test1",
                ConfirmPassword = "test1",
                BirthDate = DateTime.Parse("01/01/0001"),
                isTeacher = false
            };

            await accountsController.Create(newAccount);

            Account? addedAccount = _context.Account.Where(c => c.ID == newAccount.ID).FirstOrDefault();
            Assert.IsNotNull(addedAccount, "Add account failed");
            if(addedAccount != null)
            {
                _context.Remove(addedAccount);
            }
            await _context.SaveChangesAsync();

			UnitTestingData._account = null;
			UnitTestingData.isUnitTesting = false;
		}

        [TestMethod]
        public async Task CanCreateTeacherAccount()
        {
			UnitTestingData.isUnitTesting = true;

			AccountsController accountsController = new AccountsController(_context);
            Account newAccount = new Account
            {
                FirstName = "Unit",
                LastName = "Test Teacher",
                Email = "unittest@gmail.com",
                Password = "test1",
                ConfirmPassword = "test1",
                BirthDate = DateTime.Parse("01/01/0001"),
                isTeacher = true
            };

            await accountsController.Create(newAccount);

            Account? addedAccount = _context.Account.Where(c => c.ID == newAccount.ID).FirstOrDefault();
            Assert.IsNotNull(addedAccount, "Add account failed");
            if (addedAccount != null)
            {
                _context.Remove(addedAccount);
            }
            await _context.SaveChangesAsync();

			UnitTestingData._account = null;
			UnitTestingData.isUnitTesting = false;
		}

        [TestMethod]
        public async Task AccountAgeMustBeAtLeast18Years()
        {
			UnitTestingData.isUnitTesting = true;

			AccountsController accountsController = new AccountsController(_context);
            Account newAccount = new Account
            {
                FirstName = "Unit",
                LastName = "Test Student",
                Email = "unittest@gmail.com",
                Password = "test1",
                ConfirmPassword = "test1",
                BirthDate = DateTime.Now,
                isTeacher = false
            };

            await accountsController.Create(newAccount);

            Account? addedAccount = _context.Account.Where(c => c.ID == newAccount.ID).FirstOrDefault();
            Assert.IsNull(addedAccount, "Account with invalid age created");
            if (addedAccount != null)
            {
                _context.Remove(addedAccount);
            }
            await _context.SaveChangesAsync();

			UnitTestingData._account = null;
			UnitTestingData.isUnitTesting = false;
		}

        [TestMethod]
        public async Task CannotShareEmail()
        {
			UnitTestingData.isUnitTesting = true;

			AccountsController accountsController = new AccountsController(_context);
            Account newAccount = new Account
            {
                FirstName = "Unit",
                LastName = "Test Student",
                Email = "teststudent@gmail.com",
                Password = "test1",
                ConfirmPassword = "test1",
                BirthDate = DateTime.Parse("01/01/0001"),
                isTeacher = false
            };

            await accountsController.Create(newAccount);

            Account? addedAccount = _context.Account.Where(c => c.ID == newAccount.ID).FirstOrDefault();
            Assert.IsNull(addedAccount, "Two Accounts can share an email");
            if (addedAccount != null)
            {
                _context.Remove(addedAccount);
            }
            await _context.SaveChangesAsync();

            UnitTestingData._account = null;
			UnitTestingData.isUnitTesting = false;
		}
    }
}
