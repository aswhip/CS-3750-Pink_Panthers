using System.ComponentModel.DataAnnotations.Schema;

namespace Pink_Panthers_Project.Models
{
	public class TeachingClass
	{
		public int ID { get; set; }
		public int accountID { get; set; }
		public int classID { get; set; }

		[NotMapped]
		public Account? Account { get; set; }
	}
}
