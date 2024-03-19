using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sport.Common;

namespace Sport.Data
{
	public class TeamData
	{
		public int TeamId { get; set; }
		public DateTime RegistrationDate { get; set; }
		public int Status { get; set; }
		public SimpleData Championship { get; set; }
		public SimpleData Category { get; set; }
		public SimpleData Sport { get; set; }
	}
}
