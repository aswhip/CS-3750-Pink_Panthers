using System;
using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Controllers;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;

namespace Pink_Panthers_Test
{
    [TestClass]
    public class SubmitAssignmentTests : Tests
	{
        [TestMethod]
        public async Task StudentCanSubmitTextAssignment()
		{
            UnitTestingData.isUnitTesting = true;

            ProfileController profileController = new ProfileController(_context);
            ClassController classController = new ClassController(_context);
            var submitAssignmentController = new ClassController(_context);


            Account? account = _context.Account.Where(c => c.ID == 7).FirstOrDefault();
            UnitTestingData._account = account;

            if (account != null)
            {

                Assignment newAssignment = new Assignment
                {
                    ClassID = 28, //Data Analytics
                    AssignmentName = "Unit Test Assignment",
                    DueDate = DateTime.Now.AddDays(2),
                    PossiblePoints = 100,
                    Description = "Unit Test Assignment Description",
                    SubmissionType = "text",
                };

                //Add functionality to test if student can submit assignment??????????
                await classController.CreateAssignment(newAssignment);

                StudentSubmission newSubmission = new StudentSubmission
                {
                    AssignmentID = newAssignment.Id, // Use the ID of the created assignment
                    Submission = "Student's submission content",
                    // Set other properties as needed
                };

                await classController.SubmitAssignment(newSubmission, null);

                var submittedAssignment = await _context.StudentSubmissions
                    .FirstOrDefaultAsync(sub => sub.AssignmentID == newAssignment.Id && sub.AccountID == account.ID);

                Assert.IsNotNull(submittedAssignment, "Submission should be saved in the database.");

                // Cleanup: Remove the assignment from the database
                _context.StudentSubmissions.Remove(submittedAssignment);
                _context.Assignments.Remove(newAssignment);
                await _context.SaveChangesAsync();
            }

            profileController.logoutAccount();

        }
	}
}

