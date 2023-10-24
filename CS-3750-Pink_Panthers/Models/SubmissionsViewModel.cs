namespace Pink_Panthers_Project.Models
{
	public class SubmissionsViewModel
	{
		public Account? Account { get; set; }
		public List<StudentSubmission>? StudentSubmissions { get; set;}

		public string? AssignmentName { get; set;}
		public List<Assignment>? UpcomingAssignments { get; set;}

		public List<int>? Grades { get; set;}

		public int? MaxGrade { get; set;}
	}
}
