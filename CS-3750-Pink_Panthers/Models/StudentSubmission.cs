﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pink_Panthers_Project.Models
{
	public class StudentSubmission
	{
		public int ID { get; set; }

		public int AssignmentID { get; set; }

		public int AccountID { get; set; }

		public string? Submission { get; set; }

		[NotMapped]
		public Assignment? currentAssignment { get; set; }
		[NotMapped]
		public Account? studentAccount { get; set; }
	}
}
