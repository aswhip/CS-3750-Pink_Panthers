using Azure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Controllers;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pink_Panthers_Test
{
    [TestClass]
    public class LoginTests : Tests //Derived from Tests, can access protected member _context
    {
        [TestMethod]
        public void CanLoginToExistingAccount()
        {
			UnitTestingData.isUnitTesting = true;

			AccountsController controller = new AccountsController(_context);
            Account loginAccount = new Account
            {
                Email = "teststudent@gmail.com",
                Password = "test1"
            };
            controller.Login(loginAccount);
            Assert.IsNotNull(UnitTestingData._account);
            UnitTestingData._account = null;
            UnitTestingData.isUnitTesting = false;
        }

        [TestMethod]
        public void CantLoginToFakeAccount()
        {
			UnitTestingData.isUnitTesting = true;

			AccountsController controller = new AccountsController(_context);
            Account loginAccount = new Account
            {
                Email = "thisemaildoesntexists@gmail.com",
                Password = "test1"
            };
            controller.Login(loginAccount);
            Assert.IsNull(UnitTestingData._account);
			UnitTestingData.isUnitTesting = false;
		}

        [TestMethod]
        public void CantLoginWithInvalidPassword()
        {
			UnitTestingData.isUnitTesting = true;

			AccountsController controller = new AccountsController(_context);
            Account loginAccount = new Account
            {
                Email = "teststudent@gmail.com",
                Password = "wrongpassword"
            };
            controller.Login(loginAccount);
			Assert.IsNull(UnitTestingData._account);
			UnitTestingData.isUnitTesting = false;
		}
    }
}
