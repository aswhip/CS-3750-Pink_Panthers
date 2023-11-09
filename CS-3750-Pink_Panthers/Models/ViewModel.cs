namespace Pink_Panthers_Project.Models
{
	public class ViewModel
	{
		/*
		 * Common Model Items
		 * -----------------------------------------------------------------
		*/
		public Account? Account {  get; set; }
		public Class? Class { get; set; }
		public List<Class>? Classes { get; set; }
		public List<Assignment>? AllAssignments { get; set; }
		public List<Assignment>? Assignments { get; set; }
		public List<StudentSubmission>? StudentSubmissions { get; set; }
		public List<Class>? RegisteredCourses {  get; set; }
		public List<Notification>? Notifications { get; set; }

		/*
		 * SubmissionsViewModel Stuff
		 * -----------------------------------------------------------------
		*/

		public string? AssignmentName { get; set; }

		public List<int>? Grades { get; set; }
		public List<String>? LetterGrades { get; set; }

		public int? MaxGrade { get; set; }

		/*
		 * RegisterView Stuff
		 * ------------------------------------------------------------------
		*/

		public List<RegisteredClass>? RegisteredClasses { get; set; }
		public string? TeacherName { get; set; }

		/*
		 * ClassViewModel Stuff
		 * ------------------------------------------------------------------
		*/

		public List<StudentClassGrade>? StudentClassGrades { get; set; }

		public int countE { get; set; }
		public int countDPlus { get; set; }
		public int countD { get; set; }
		public int countDMinus { get; set; }
		public int countCPlus { get; set; }
		public int countC { get; set; }
		public int countCMinus { get; set; }
		public int countBPlus { get; set; }
		public int countB { get; set; }
		public int countBMinus { get; set; }
		public int countA { get; set; }
		public int countAMinus { get; set; }

		/*
		 * CourseView Stuff
		 * ------------------------------------------------------------------
		*/

		public List<Class>? TeachingCourses { get; set; }

        /*
		 * Receipt Stuff
		 * ------------------------------------------------------------------
		*/

        public decimal AmountToPay { get; set; }

		/**
		 * Grade View Stuff
		 * ------------------------------------------------------------------
		*/

		public List<RGB>? GraphColor {  get; set; }

    }
}
