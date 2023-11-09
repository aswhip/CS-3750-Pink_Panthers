using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Controllers;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;

namespace Pink_Panthers_Test
{
    [TestClass]
    public class DeleteClassTests : Tests
    {
        [TestMethod]
        public async Task TeacherCanDeleteClassAsync()
        {
            UnitTestingData.isUnitTesting = true;

            ClassController classController = new ClassController(_context);
            ProfileController profileController = new ProfileController(_context);
            Class newClass = new Class
            {
                Room = "NB318",
                DepartmentCode = "TS",
                CourseNumber = "9998",
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

            await profileController.addClass(newClass);
            int count = _context.teachingClasses.Where(c => c.accountID == account!.ID).Count();
            await classController.DeleteCourse(newClass.ID);
            int newCount = _context.teachingClasses.Where(c => c.accountID == account!.ID).Count();

            Assert.AreEqual(newCount, count - 1, "Delete class failed");

            profileController.logoutAccount();
        }
    }
}