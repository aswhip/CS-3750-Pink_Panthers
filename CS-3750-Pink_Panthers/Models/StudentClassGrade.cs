namespace Pink_Panthers_Project.Models
{
	public class StudentClassGrade
	{
		public int StudentID { get; set; }
		public int? StudentPointsEarned { get; set; }
		public int? TotalPossiblePoints { get; set; }

		public string? StudentGrade { get; set; }

	}
}