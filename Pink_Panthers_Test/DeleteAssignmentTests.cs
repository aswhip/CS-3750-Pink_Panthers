using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pink_Panthers_Project.Controllers;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;

namespace Pink_Panthers_Test
{
    [TestClass]
    public class DeleteAssignmentTests: Tests
    {
        [TestMethod]
        public async Task TeacherCanDeleteAssignment() 
        {
            UnitTestingData.isUnitTesting = true;

            ClassController classController = new ClassController(_context);
            ProfileController profileController = new ProfileController(_context);

            Assignment newAssignment = new Assignment
            {
                AssignmentName = "UNIT TEST ASSIGNMENT",
                DueDate = DateTime.Parse("12/12/2023"),
                PossiblePoints = 100,
                Description = "UNIT TEST DESCRIPTION",
                SubmissionType = "text",
                ClassID = 11
            };
            Account? account = _context.Account.Where(c => c.ID == 6).FirstOrDefault(); //Teacher Account

            UnitTestingData._account = account;

            await classController.CreateAssignment(newAssignment);
            int count = _context.Assignments.Where(c => c.ClassID == newAssignment.ClassID).Count();
            await classController.DeleteAssignment(newAssignment.Id);
            int newCount = _context.Assignments.Where(c => c.ClassID == newAssignment.ClassID).Count();

            Assert.AreEqual(newCount, count - 1, "Delete assignment failed");

            profileController.logoutAccount();

        }
    }
}
