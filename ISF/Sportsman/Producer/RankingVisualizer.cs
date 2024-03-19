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

		private bool _competitionType = false;
		public RankingVisualizer(bool blnCompetition)
		{
			_competitionType = blnCompetition;
			Fields.Add(new Sport.UI.VisualizerField("מיקום", 20, System.Drawing.StringAlignment.Center));
			Fields.Add(new Sport.UI.VisualizerField("קבוצה", 75));
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

			if (_competitionType && _rankingTable != null)
				Fields.Add(new Sport.UI.VisualizerField("עמידה בתקנון", 25, System.Drawing.StringAlignment.Center));
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
			if (_competitionType &&  field == (this.Fields.Count - 1))
			{
				if (_rankingTable != null)
				{
					bool blnValid = true;
					for (int n = 2; n < _rankingTable.Fields.Count; n++)
					{
						string strTitle = this.Fields[n].Title;
						Sport.Common.EquationVariables var = new Sport.Common.EquationVariables();
						team.SetFields(var);
						string strText = _rankingTable.Fields[n - 2].Evaluate(var);
						if (strTitle.IndexOf("נוספות") < 0 && strTitle != "נקודות" && strText != "???")
						{
							if (Sport.Common.Tools.IsInteger(strText) && Int32.Parse(strText) <= 0)
							{
								blnValid = false;
								break;
							}
						}
					}
					return (blnValid) ? "כן" : "לא";
				}
			}
			
			switch (field)
			{
				case (int) RankingField.Position:
					return (team.Position + 1).ToString();
				case (int) RankingField.Team:
					return team.Name;
				default: // RankFields rule field
					if (_rankingTable != null && field > 1 && field - 2 < _rankingTable.Fields.Count)
					{
						string title = this.Fields[field].Title;
						Sport.Common.EquationVariables var = new Sport.Common.EquationVariables();
						team.SetFields(var);
						string text = _rankingTable.Fields[field - 2].Evaluate(var);
						if (_competitionType && title.IndexOf("נוספות") < 0 && title != "נקודות" && text != "???")
						{
							if (Sport.Common.Tools.IsInteger(text) && Int32.Parse(text) <= 0)
							{
								text += ":::202,202,202";
							}
						}
						return text;
					}
					break;
			}

			return null;
		}
	}
}
