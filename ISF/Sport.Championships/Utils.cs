using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sport.Championships
{
	public static class ChampionshipUtils
	{
		private static Dictionary<int, int> RegionMapping = new Dictionary<int, int>();
		private static Dictionary<string, string> SportFieldMapping = new Dictionary<string, string>();

		static ChampionshipUtils()
		{
			RegionMapping.Add(0, 9); //מטה
			SportFieldMapping.Add("כדורסל", "B");
			SportFieldMapping.Add("כדורגל", "F");
			SportFieldMapping.Add("קטרגל", "F");
			SportFieldMapping.Add("כדוריד", "H");
			SportFieldMapping.Add("כדורעף", "V");
		}

		public static string BuildReferreOrderNumber(Match match)
		{
			string orderNumber = "";
			Sport.Entities.Championship championship = match.Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Championship;
			int regionID = championship.Region.Id;
			string sportFieldName = championship.Sport.Name;
			if (RegionMapping.ContainsKey(regionID))
				regionID = RegionMapping[regionID];
			string matchingKey = SportFieldMapping.Keys.OfType<string>().ToList().Find(k => sportFieldName.Contains(k));
			string sportFieldLetter = (matchingKey != null && matchingKey.Length > 0) ? SportFieldMapping[matchingKey] : "";
			if (sportFieldLetter.Length > 0)
				orderNumber = regionID.ToString() + sportFieldLetter + match.Time.Year.ToString();
			return orderNumber;
		}
	}
}
