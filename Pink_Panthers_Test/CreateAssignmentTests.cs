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
	public class CreateAssignmentTests : Tests //Derived from Tests, can access protected member _context
	{
		[TestMethod]
		public async Task TeacherCanCreateAssignment()
		{
			UnitTestingData.isUnitTesting = true;

			ProfileController profileController = new ProfileController(_context);

			ClassController classController = new ClassController(_context);

			Account? account = _context.Account.Where(c => c.ID == 6).FirstOrDefault(); //Teacher Account
			UnitTestingData._account = account;

			Assignment newAssignment = new Assignment
			{
				ClassID = 6, //Unit Test Course
				AssignmentName = "Unit Test Assignment",
				DueDate = DateTime.Now.AddDays(2),
				PossiblePoints = 100,
				Description = "Unit Test Assignment Description",
				SubmissionType = "text",
			};

			int count = _context.Assignments.Where(c => c.ClassID == newAssignment.ClassID).Count();
			await classController.CreateAssignment(newAssignment);

			int newCount = _context.Assignments.Where(c => c.ClassID == newAssignment.ClassID).Count();

			Assert.AreEqual(newCount, count + 1, "Create Assignement failed");




			var addedAssignment = _context.Assignments.FirstOrDefault(a => a.AssignmentName == "Unit Test Assignment"  && a.ClassID == 6);

			if (addedAssignment != null)
			{
				_context.Assignments.Remove(addedAssignment);
				await _context.SaveChangesAsync();
			}

			profileController.logoutAccount();
		}

	}
}
