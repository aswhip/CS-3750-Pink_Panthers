using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pink_Panthers_Project.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pink_Panthers_Test.Data
{
    internal class Pink_Panthers_TestContext : DbContext
    {
        public Pink_Panthers_TestContext(DbContextOptions<Pink_Panthers_TestContext> options) : base(options) { }

        public DbSet<Pink_Panthers_Project.Models.Account> Account { get; set; } = default!;
        public DbSet<Pink_Panthers_Project.Models.Class> Class { get; set; } = default!;
        public DbSet<Pink_Panthers_Project.Models.RegisteredClass> registeredClasses { get; set; } = default!;
        public DbSet<Pink_Panthers_Project.Models.Assignment> Assignments { get; set; } = default!;
    }
}
