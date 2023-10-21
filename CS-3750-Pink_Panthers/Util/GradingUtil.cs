using System;
namespace Pink_Panthers_Project.Util
{
    public static class GradingUtil
    {
        public static string GetLetterGrade(double numericGrade)
        {
            if (numericGrade >= 94.00)
            {
                return "A";
            }
            else if (numericGrade >= 90.00)
            {
                return "A-";
            }
            else if (numericGrade >= 87.00)
            {
                return "B+";
            }
            else if (numericGrade >= 84.00)
            {
                return "B";
            }
            else if (numericGrade >= 80.00)
            {
                return "B-";
            }
            else if (numericGrade >= 77.00)
            {
                return "C+";
            }
            else if (numericGrade >= 74.00)
            {
                return "C";
            }
            else if (numericGrade >= 70.00)
            {
                return "C-";
            }
            else if (numericGrade >= 67.00)
            {
                return "D+";
            }
            else if (numericGrade >= 64.00)
            {
                return "D";
            }
            else if (numericGrade >= 60.00)
            {
                return "D-";
            }
            else
            {
                return "E";
            }
        }

    }
}
