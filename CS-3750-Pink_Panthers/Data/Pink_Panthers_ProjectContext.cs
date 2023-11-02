using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Models;

namespace Pink_Panthers_Project.Data
{
    public class Pink_Panthers_ProjectContext : DbContext
    {
        public Pink_Panthers_ProjectContext (DbContextOptions<Pink_Panthers_ProjectContext> options)
            : base(options)
        {
        }

        public DbSet<Pink_Panthers_Project.Models.Account> Account { get; set; } = default!;
        public DbSet<Pink_Panthers_Project.Models.Class> Class { get; set; } = default!;
        public DbSet<Pink_Panthers_Project.Models.RegisteredClass> registeredClasses { get; set; } = default!;
        public DbSet<Pink_Panthers_Project.Models.TeachingClass> teachingClasses { get; set; } = default!;
        public DbSet<Pink_Panthers_Project.Models.Assignment> Assignments { get; set; } = default!;
        public DbSet<Pink_Panthers_Project.Models.StudentSubmission> StudentSubmissions { get; set;} = default!;
        public DbSet<Pink_Panthers_Project.Models.Notification> Notifications { get; set; } = default!;
    }
}
