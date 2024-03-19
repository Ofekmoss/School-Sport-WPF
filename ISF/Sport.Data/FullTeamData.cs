using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sport.Common;

namespace Sport.Data
{
	public class FullTeamData
	{
		public int Id { get; set; }
		public int TeamStatus { get; set; }
		public int ChampionshipStatus { get; set; }
		public int Index { get; set; }
		public DateTime RegistrationDate { get; set; }
		public SchoolData School { get; set; }
		public SimpleData City { get; set; }
		public ChampionshipCategoryData Category { get; set; }
		public SimpleData Championship { get; set; }
		public SimpleData Sport { get; set; }
		public SimpleData Region { get; set; }
		public List<PlayerData> Players { get; set; }
	}
}
