using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Models;
using Pink_Panthers_Test.Data;

namespace Pink_Panthers_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TeacherCanCreateClass()
        {
            DbContextOptions<Pink_Panthers_TestContext> options = new DbContextOptions<Pink_Panthers_TestContext>();
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
            SqlServerDbContextOptionsExtensions.UseSqlServer(builder, "Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_PinkPanthers;User ID=3750_PinkPanthers;Password=P1nkpanthers!;TrustServerCertificate=True");
            var _context = new Pink_Panthers_TestContext((DbContextOptions<Pink_Panthers_TestContext>)builder.Options);
            _context.Database.EnsureCreated();

            //Count how many classes teacher has (n classes)
            int count = _context.Class.Where(c => c.accountID == 5).Count();

            //Create new class
            Class newClass = new Class
            {
                accountID = 5, //accountID 5 is Test Teacher
                Room = "NB318",
                DepartmentCode = "CS",
                CourseNumber = "1410",
                CourseName = "Object Oriented Programming",
                color = $"#0000FF",
                Days = "M W F",
                StartTime = DateTime.Parse("9:30 AM"),
                EndTime = DateTime.Parse("11:00 AM")
            };

            //Temporarily adding the class
            _context.Add(newClass);
            _context.SaveChanges();

            int newcount = _context.Class.Where(c => c.accountID == 5).Count(); //Getting the new value
                
            Assert.AreEqual(newcount, count + 1); //Should have 1 extra class now

            //Delete class
            _context.Remove(newClass);
            _context.SaveChanges();
            
            

        }
    }
}