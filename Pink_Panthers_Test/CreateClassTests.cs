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
            ProfileController profileController = new ProfileController(_context);
            Class newClass = new Class
            {
                Room = "NB318",
                DepartmentCode = "CS",
                CourseNumber = "1410",
                CourseName = "Object Oriented Programming",
                monday = true,
                tuesday = false,
                wednesday = true,
                thursday = false,
                friday = true,
                StartTime = DateTime.Parse("9:30 AM"),
                EndTime = DateTime.Parse("11:00 AM")
            };
            Account? account = _context.Account.Where(c => c.ID == 5).FirstOrDefault(); //Teacher Account
            ProfileController.setAccount(ref account!);

            int count = _context.Class.Where(c => c.accountID == account.ID).Count();
            await profileController.addClass(newClass);
            int newCount = _context.Class.Where(c => c.accountID == account.ID).Count();

            Assert.AreEqual(newCount, count + 1, "Add class failed");

            _context.Class.Remove(newClass);
            await _context.SaveChangesAsync();

            ProfileController.logoutAccount();
        }

        [TestMethod]
        public async Task StudentCannotCreateClass()
        {
            ProfileController profileController = new ProfileController(_context);
            Class newClass = new Class
            {
                Room = "NB318",
                DepartmentCode = "CS",
                CourseNumber = "1410",
                CourseName = "Object Oriented Programming",
                monday = true,
                tuesday = false,
                wednesday = true,
                thursday = false,
                friday = true,
                StartTime = DateTime.Parse("9:30 AM"),
                EndTime = DateTime.Parse("11:00 AM")
            };
            Account? account = _context.Account.Where(c => c.ID == 1).FirstOrDefault(); //Student Account
            ProfileController.setAccount(ref account!);

            int count = _context.Class.Where(c => c.accountID == account.ID).Count();
            Assert.AreEqual(count, 0, "Count should be 0");

            await profileController.addClass(newClass);
            int newCount = _context.Class.Where(c => c.accountID == account.ID).Count();

            Assert.AreEqual(newCount, count, "Student added class"); //Student should not be able to add class, so new count and count should be equal

            await _context.SaveChangesAsync();

            ProfileController.logoutAccount();
        }
    }
}