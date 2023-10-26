namespace Pink_Panthers_Project.Models
{
    public class ClassViewModel
    {
        public Account? Account { get; set; }

        public Class? Class { get; set; }

        public List<Assignment>? Assignments { get; set; }

        public List<StudentSubmission>? StudentSubmissions { get; set; } 
        public List<Assignment>? UpcomingAssignments { get; set; }

		public List<StudentClassGrade>? StudentClassGrades { get; set; }

        public int countE {  get; set; }
        public int countDPlus { get; set; }
        public int countD { get; set; }
        public int countDMinus { get; set; }
        public int countCPlus { get; set; }
        public int countC { get; set; }
        public int countCMinus {  get; set; }
        public int countBPlus {  get; set; }
        public int countB { get; set; }
        public int countBMinus { get; set; }
        public int countA {  get; set; }
        public int countAMinus {  get; set; }

    }
}
