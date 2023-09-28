namespace Pink_Panthers_Project.Models
{
    public class CourseView
    {
        public List<Class>? TeachingCourses { get; set; }
        public List<Class>? RegisteredCourses { get; set; }
        public List<Assignment>? Assignments { get; set; }
        public Account? Account { get; set; }
    }
}
