using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sport.Data
{
	public class ChampionshipData
	{
		public int SportId { get; set; }
		public string SportName { get; set; }
		public int ChampionshipId { get; set; }
		public string ChampionshipName { get; set; }
		public int RegionId { get; set; }
		public string RegionName { get; set; }
		public ChampionshipCategoryData[] Categories { get; set; }
	}

	public class ChampionshipCategoryData
	{
		public int Id { get; set; }
		public int Category { get; set; }
		public string Name { get; set; }
	}
}
