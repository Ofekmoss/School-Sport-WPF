using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sport.Common;

namespace Sport.Data
{
	public class PlayerData
	{
		public int Id { get; set; }
		public int Status { get; set; }
		public int TeamNumber { get; set; }
		public StudentData Student { get; set; }
		public SimpleData Grade { get; set; }
	}
}
