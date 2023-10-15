namespace Pink_Panthers_Project.Models
{
    public class ClassViewModel
    {
        public Account? Account { get; set; }

        public Class? Class { get; set; }

        public List<Assignment>? Assignments { get; set; }

        public List<StudentSubmission>? StudentSubmissions { get; set; } 
        public List<Assignment>? UpcomingAssignments { get; set; }
    }
}
