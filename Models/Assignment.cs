using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pink_Panthers_Project.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public int ClassID { get; set; }
        public string? AssignmentName { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [NotMapped]
        public string? className;
    }
}
