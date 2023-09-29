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
    public class LoginTests
    {
        private readonly Pink_Panthers_ProjectContext _context;
        public LoginTests()
        {
            DbContextOptions<Pink_Panthers_ProjectContext> options = new DbContextOptions<Pink_Panthers_ProjectContext>();
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
            SqlServerDbContextOptionsExtensions.UseSqlServer(builder, "Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_PinkPanthers;User ID=3750_PinkPanthers;Password=P1nkpanthers!;TrustServerCertificate=True");
            _context = new Pink_Panthers_ProjectContext((DbContextOptions<Pink_Panthers_ProjectContext>)builder.Options);
        }

        [TestMethod]
        public void CanLoginToExistingAccount()
        {
            AccountsController accountsController = new AccountsController(_context);
            Account loginAccount = new Account
            {
                Email = "teststudent@gmail.com",
                Password = "test1"
            };
            accountsController.Login(loginAccount);
            Assert.IsNotNull(ProfileController.getAccount());
            ProfileController.logoutAccount(); //Logout once we are done
        }

        [TestMethod]
        public void CantLoginToFakeAccount()
        {
            AccountsController accountsController = new AccountsController(_context);
            Account loginAccount = new Account
            {
                Email = "thisemaildoesntexists@gmail.com",
                Password = "test1"
            };
            accountsController.Login(loginAccount);
            Assert.IsNull(ProfileController.getAccount());
            ProfileController.logoutAccount();
        }

        [TestMethod]
        public void CantLoginWithInvalidPassword()
        {
            AccountsController accountsController = new AccountsController(_context);
            Account loginAccount = new Account
            {
                Email = "teststudent@gmail.com",
                Password = "wrongpassword"
            };
            accountsController.Login(loginAccount);
            Assert.IsNull(ProfileController.getAccount());
            ProfileController.logoutAccount();
        }
    }
}
