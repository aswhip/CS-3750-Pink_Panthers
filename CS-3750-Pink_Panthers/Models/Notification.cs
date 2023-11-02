using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pink_Panthers_Project.Models
{
	public class Notification
	{
		public int Id { get; set; }
		public int StudentId { get; set; }
		public int AssignmentId { get; set; }
		public string NotificationString { get; set; } = "";

		public bool IsCleared { get; set; }


		[NotMapped]
		public string? AssignmentName { get; set; }
		[NotMapped]
		public string? ClassName { get; set; }
		[NotMapped]
		[DataType(DataType.Date)]
		public DateTime DueDate { get; set; }

	}
}
