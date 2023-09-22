using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pink_Panthers_Project.Models
{
    public class Class
    {
        public int ID { get; set; }
        public int accountID { get; set; }
        public string? Room {  get; set; }
        public string? DepartmentCode { get; set; }
        public string? CourseNumber { get; set; }
        public string? CourseName { get; set; }

        public string? Days { get; set; }

        [DataType(DataType.Time)]
        public DateTime StartTime {  get; set; }
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
    }
}
