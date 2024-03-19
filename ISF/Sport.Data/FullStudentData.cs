using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sport.Common;

namespace Sport.Data
{
	public enum FullStudentType
	{
		Player = 1,
		Pending,
		School
	}

	public class FullStudentData
	{
		public int Id { get; set; }
		public FullStudentType Type { get; set; }
		public int Grade { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string IdNumber { get; set; }
		public int SchoolId { get; set; }
		public DateTime BirthDate { get; set; }
	}
}
