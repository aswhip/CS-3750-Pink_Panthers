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
    public class Tests //Base Class for Tests, implements _context
    {
        protected readonly Pink_Panthers_ProjectContext _context;
        public Tests()
        {
            DbContextOptions<Pink_Panthers_ProjectContext> options = new DbContextOptions<Pink_Panthers_ProjectContext>();
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
            SqlServerDbContextOptionsExtensions.UseSqlServer(builder, "Server=tcp:pinkpanthers.database.windows.net,1433;Initial Catalog=3750_PinkPanthers;Persist Security Info=False;User ID=PinkPanthers;Password=P1nkpanthers!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            _context = new Pink_Panthers_ProjectContext((DbContextOptions<Pink_Panthers_ProjectContext>)builder.Options);
        }
    }

}
