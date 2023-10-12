using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pink_Panthers_Project.Models
{
    public class Class
    {
        public int ID { get; set; }
        public string? Room { get; set; }
        public string? DepartmentCode { get; set; }
        public string? CourseNumber { get; set; }
        public string? CourseName { get; set; }
        public string? color { get; set; }
        public int hours { get; set; }

        public string? Days { get; set; }

        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }

        [NotMapped]
        public bool monday { get; set; }
        [NotMapped]
        public bool tuesday { get; set; }
        [NotMapped]
        public bool wednesday { get; set; }
        [NotMapped]
        public bool thursday { get; set; }
        [NotMapped]
        public bool friday { get; set; }
        [NotMapped]
        public string? tName { get; set; }        
    }
}
