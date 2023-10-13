using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Controllers;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;

namespace Pink_Panthers_Test
{
    [TestClass]
    public class CreateClassTests : Tests //Derived from Tests, can access protected member _context
    {

        [TestMethod]
        public async Task TeacherCanCreateClassAsync()
        {
			UnitTestingData.isUnitTesting = true;

			ProfileController profileController = new ProfileController(_context);
            Class newClass = new Class
            {
                Room = "NB318",
                DepartmentCode = "TS",
                CourseNumber = "9999",
                CourseName = "UNIT TEST COURSE",
                monday = true,
                tuesday = false,
                wednesday = true,
                thursday = false,
                friday = true,
                StartTime = DateTime.Parse("9:30 AM"),
                EndTime = DateTime.Parse("11:00 AM")
            };
            Account? account = _context.Account.Where(c => c.ID == 6).FirstOrDefault(); //Teacher Account

            UnitTestingData._account = account;

            int count = _context.teachingClasses.Where(c => c.accountID == account!.ID).Count();
            await profileController.addClass(newClass);
            int newCount = _context.teachingClasses.Where(c => c.accountID == account!.ID).Count();

            Assert.AreEqual(newCount, count + 1, "Add class failed");

            _context.Class.Remove(newClass);
            await _context.SaveChangesAsync();

            profileController.logoutAccount();
        }

        [TestMethod]
        public async Task StudentCannotCreateClass()
        {
			UnitTestingData.isUnitTesting = true;

			ProfileController profileController = new ProfileController(_context);
            Class newClass = new Class
            {
                Room = "NB318",
                DepartmentCode = "TS",
                CourseNumber = "9999",
                CourseName = "UNIT TEST COURSE",
                monday = true,
                tuesday = false,
                wednesday = true,
                thursday = false,
                friday = true,
                StartTime = DateTime.Parse("9:30 AM"),
                EndTime = DateTime.Parse("11:00 AM")
            };
            Account? account = _context.Account.Where(c => c.ID == 5).FirstOrDefault(); //Student Account

            UnitTestingData._account = account;

            int count = _context.teachingClasses.Where(c => c.accountID == account!.ID).Count();
            Assert.AreEqual(count, 0, "Count should be 0");

            await profileController.addClass(newClass);
            int newCount = _context.teachingClasses.Where(c => c.accountID == account!.ID).Count();

            Assert.AreEqual(newCount, count, "Student added class"); //Student should not be able to add class, so new count and count should be equal

            await _context.SaveChangesAsync();

            profileController.logoutAccount();
        }
    }
}