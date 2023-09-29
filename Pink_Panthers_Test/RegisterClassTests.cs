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
    public class RegisterClassTests
    {
        private readonly Pink_Panthers_ProjectContext _context;
        public RegisterClassTests()
        {
            DbContextOptions<Pink_Panthers_ProjectContext> options = new DbContextOptions<Pink_Panthers_ProjectContext>();
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
            SqlServerDbContextOptionsExtensions.UseSqlServer(builder, "Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_PinkPanthers;User ID=3750_PinkPanthers;Password=P1nkpanthers!;TrustServerCertificate=True");
            _context = new Pink_Panthers_ProjectContext((DbContextOptions<Pink_Panthers_ProjectContext>)builder.Options);
        }

        [TestMethod]
        public async Task StudentCanRegisterForClass()
        {
            ProfileController profileController = new ProfileController(_context);
            Account? account = _context.Account.Where(c => c.ID == 1).FirstOrDefault(); //ID 1 is test student

            if(account != null)
                ProfileController.setAccount(ref account!);

            await profileController.Register(7); //Class ID 7 is the Test Course
            var didRegister = _context.registeredClasses.Where(c => c.classID == 7 && c.accountID == account!.ID).FirstOrDefault();
            Assert.IsNotNull(didRegister);
            if(didRegister != null)
            {
                _context.Remove(didRegister);
            }

            await _context.SaveChangesAsync();

            ProfileController.logoutAccount(); //Logout since we are done using it now
        }

        [TestMethod]
        public async Task StudentCanDropRegisteredClass()
        {
            //Need to temporarily register for a class
            ProfileController profileController = new ProfileController(_context);
            Account? account = _context.Account.Where(c => c.ID == 1).FirstOrDefault(); //ID 1 is test student

            if (account != null)
                ProfileController.setAccount(ref account!);

            await profileController.Register(7); //Class ID 7 is the Test Course

            //Now we need to drop the class, which uses the same method but drops the class if they are already registered
            await profileController.Register(7); //Drops the class 

            var didDrop = _context.registeredClasses.Where(c => c.classID == 7 && c.accountID == account!.ID).FirstOrDefault();

            Assert.IsNull(didDrop, "Class Drop Failed");

            if (didDrop != null)
            {
                _context.Remove(didDrop);
            }

            await _context.SaveChangesAsync();

            ProfileController.logoutAccount(); //Logout since we are done using it now
        }
    }
}
