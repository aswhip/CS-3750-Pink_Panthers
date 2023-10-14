using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pink_Panthers_Project.Models
{
    public class Assignment
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
        public int ClassID { get; set; }

		[Required(ErrorMessage = "Please provide a name for the assignment.")]
		[StringLength(30, ErrorMessage = "Must contain at least 2 characters", MinimumLength = 2)]
		public string? AssignmentName { get; set; }

        [DataType(DataType.Date)]
		[Required(ErrorMessage = "Please provide a due date")]
		public DateTime DueDate { get; set; }

        [NotMapped]
		public string? className { get; set; }


		[Required(ErrorMessage = "Please provide a number")]
		[Range(1, int.MaxValue, ErrorMessage = "Points has to be higher than 0")]
		public int PossiblePoints { get; set; }

        public string? Description { get; set; }

        [Required]
        public string? SubmissionType { get; set; }

        [NotMapped]
        public int? grade { get; set; }

        [NotMapped]
        public bool? submitted { get; set; }
    }
}
