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
    public class RegisterClassTests : Tests //Derived from Tests, can access protected member _context
    {
        [TestMethod]
        public async Task StudentCanRegisterForClass()
        {
			UnitTestingData.isUnitTesting = true;

			ProfileController profileController = new ProfileController(_context);
            Account? account = _context.Account.Where(c => c.ID == 5).FirstOrDefault(); //ID 5 is test student

            if(account != null)
                UnitTestingData._account = account!;

            await profileController.Register(6); //Class ID 6 is the Test Course
            var didRegister = _context.registeredClasses.Where(c => c.classID == 6 && c.accountID == account!.ID).FirstOrDefault();
            Assert.IsNotNull(didRegister);
            if(didRegister != null)
            {
				await profileController.Register(6); //Dropping the course
			}

            await _context.SaveChangesAsync();

            profileController.logoutAccount(); //Logout since we are done using it now
        }

        [TestMethod]
        public async Task StudentCanDropRegisteredClass()
        {
			UnitTestingData.isUnitTesting = true;

			//Need to temporarily register for a class
			ProfileController profileController = new ProfileController(_context);
            Account? account = _context.Account.Where(c => c.ID == 5).FirstOrDefault(); //ID 5 is test student

            if (account != null)
				UnitTestingData._account = account!;

			await profileController.Register(6); //Class ID 6 is the Test Course

            //Now we need to drop the class, which uses the same method but drops the class if they are already registered
            await profileController.Register(6); //Drops the class 

            var didDrop = _context.registeredClasses.Where(c => c.classID == 6 && c.accountID == account!.ID).FirstOrDefault();

            Assert.IsNull(didDrop, "Class Drop Failed");

            if (didDrop != null)
            {
                _context.Remove(didDrop);
            }

            await _context.SaveChangesAsync();

            profileController.logoutAccount(); //Logout since we are done using it now
        }

        [TestMethod]
        public async Task TeacherCannotRegisterForClass()
        {
			UnitTestingData.isUnitTesting = true;

			ProfileController profileController = new ProfileController(_context);
            Account? account = _context.Account.Where(c => c.ID == 6).FirstOrDefault(); //ID 6 is test teacher

            if (account != null)
				UnitTestingData._account = account!;

			await profileController.Register(6); //Class ID 46 is the Test Course

            var didRegister = _context.registeredClasses.Where(c => c.classID == 6 && c.accountID == account!.ID).FirstOrDefault();

            Assert.IsNull(didRegister, "Teacher can register for class");

            if (didRegister != null)
            {
                _context.Remove(didRegister);
            }

            await _context.SaveChangesAsync();

            profileController.logoutAccount(); //Logout since we are done using it now
        }
    }
}
