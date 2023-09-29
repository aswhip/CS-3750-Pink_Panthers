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
    public class CreateAccountTests
    {
        private readonly Pink_Panthers_ProjectContext _context;
        public CreateAccountTests()
        {
            DbContextOptions<Pink_Panthers_ProjectContext> options = new DbContextOptions<Pink_Panthers_ProjectContext>();
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
            SqlServerDbContextOptionsExtensions.UseSqlServer(builder, "Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_PinkPanthers;User ID=3750_PinkPanthers;Password=P1nkpanthers!;TrustServerCertificate=True");
            _context = new Pink_Panthers_ProjectContext((DbContextOptions<Pink_Panthers_ProjectContext>)builder.Options);
        }

        [TestMethod]
        public async Task CanCreateStudentAccount()
        {
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

            ProfileController.logoutAccount();
        }

        [TestMethod]
        public async Task CanCreateTeacherAccount()
        {
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

            ProfileController.logoutAccount();
        }

        [TestMethod]
        public async Task AccountAgeAtLeast18Years()
        {
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

            ProfileController.logoutAccount();
        }

        [TestMethod]
        public async Task CannotShareEmail()
        {
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

            ProfileController.logoutAccount();
        }
    }
}
