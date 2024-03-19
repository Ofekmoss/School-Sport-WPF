using System;
using Sport.Rulesets.Rules;

namespace Sportsman.Producer
{
	public class RankingVisualizer : Sport.UI.Visualizer
	{
		public enum RankingField
		{
			Position = 0,
			Team
		}

		public RankingVisualizer()
		{
			Fields.Add(new Sport.UI.VisualizerField("מיקום", 20, System.Drawing.StringAlignment.Center));
			Fields.Add(new Sport.UI.VisualizerField("קבוצה", 100));
		}

		private RankingTable _rankingTable;
		public RankingTable RankingTable
		{
			get { return _rankingTable; }
			set
			{
				if (_rankingTable != value)
				{
					_rankingTable = value;
					ResetRankingTable();
				}
			}
		}

		private void ResetRankingTable()
		{
			while (Fields.Count > 2)
				Fields.RemoveAt(2);

			if (_rankingTable != null)
			{
				for (int n = 0; n < _rankingTable.Fields.Count; n++)
				{
					Fields.Add(new Sport.UI.VisualizerField(_rankingTable.Fields[n].Title, 20, System.Drawing.StringAlignment.Center));
				}
			}
		}

		public override string GetText(object o, int field)
		{
			Sport.Championships.Team team = o as Sport.Championships.Team;
			if (team != null)
				return GetText(team, field);
			return null;
		}

		public string GetText(Sport.Championships.Team team, int field)
		{
			switch (field)
			{
				case (int) RankingField.Position:
					return (team.Position + 1).ToString();
				case (int) RankingField.Team:
					return team.Name;
				default: // RankFields rule field
					if (_rankingTable != null && field > 1 && field - 2 < _rankingTable.Fields.Count)
					{
						Sport.Common.EquationVariables var = new Sport.Common.EquationVariables();
						team.SetFields(var);
						return _rankingTable.Fields[field - 2].Evaluate(var);
					}
					break;
			}

			return null;
		}
	}
}
