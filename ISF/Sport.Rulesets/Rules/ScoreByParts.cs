using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sport.Rulesets.Rules
{
	public class ScoreByParts : ICloneable
	{
		private List<ScoreByParts.Value> values = new List<Value>();

		public ScoreByParts()
		{
		}

		public ScoreByParts.Value[] Values
		{
			get
			{
				return values.ToArray();
			}
		}

		public void AddValue(ScoreByParts.Value value)
		{
			values.Add(value);
		}

		public void RemoveValue(ScoreByParts.Value value)
		{
			values.Remove(value);
		}

		public override string ToString()
		{
			return string.Join("\n", values.ToList().ConvertAll(value => value.ToString()));
		}

		public class Value : ICloneable
		{
			public int TeamA_PartScore { get; set; }
			public int TeamB_PartScore { get; set; }
			public int TeamA_GameScore { get; set; }
			public int TeamB_GameScore { get; set; }

			public override string ToString()
			{
				return string.Format("{0}-{1} | {2}-{3}", this.TeamA_PartScore, this.TeamB_PartScore,
					this.TeamA_GameScore, this.TeamB_GameScore);
			}

			public bool TryUpdate(string rawPartScore, string rawGameScore, out string errorMsg)
			{
				errorMsg = "";
				int teamA_PartScore, teamB_PartScore, teamA_GameScore, teamB_GameScore;
				if (!TryParseRawScore(rawPartScore, "ניקוד", out teamA_PartScore, out teamB_PartScore, out errorMsg) ||
					!TryParseRawScore(rawGameScore, "תוצאה", out teamA_GameScore, out teamB_GameScore, out errorMsg))
					return false;

				this.TeamA_PartScore = teamA_PartScore;
				this.TeamA_GameScore = teamA_GameScore;
				this.TeamB_PartScore = teamB_PartScore;
				this.TeamB_GameScore = teamB_GameScore;
				return true;
			}

			public object Clone()
			{
				return new Value
				{
					TeamA_PartScore = this.TeamA_PartScore,
					TeamA_GameScore = this.TeamA_GameScore,
					TeamB_PartScore = this.TeamB_PartScore,
					TeamB_GameScore = this.TeamB_GameScore
				};
			}

			public static bool TryCreate(string rawPartScore, string rawGameScore, out ScoreByParts.Value scoreByPartsItem, out string errorMsg)
			{
				scoreByPartsItem = null;
				errorMsg = "";
				int teamA_PartScore, teamB_PartScore, teamA_GameScore, teamB_GameScore;
				if (!TryParseRawScore(rawPartScore, "ניקוד", out teamA_PartScore, out teamB_PartScore, out errorMsg) ||
					!TryParseRawScore(rawGameScore, "תוצאה", out teamA_GameScore, out teamB_GameScore, out errorMsg))
					return false;

				scoreByPartsItem = new Value
				{
					TeamA_PartScore = teamA_PartScore, 
					TeamA_GameScore = teamA_GameScore, 
					TeamB_PartScore = teamB_PartScore, 
					TeamB_GameScore = teamB_GameScore
				};

				return true;
			}

			private static bool TryParseRawScore(string rawScore, string caption, out int teamA, out int teamB, out string errorMsg)
			{
				teamA = 0;
				teamB = 0;
				errorMsg = "";
				string[] parts = rawScore.Trim().Split('-');
				if (parts.Length != 2)
				{
					errorMsg = "ערך " + caption + " שגוי, פורמט לא תקין";
					return false;
				}
				if (!Int32.TryParse(parts[0], out teamA) || teamA < 0 || !Int32.TryParse(parts[1], out teamB) || teamB < 0)
				{
					errorMsg = "ערך " + caption + " לא תקין, חייב להיות שני מספרים מופרדים במקף";
					return false;
				}

				return true;
			}
		}

		public object Clone()
		{
			ScoreByParts clone = new ScoreByParts();
			clone.values = this.values.ConvertAll(v => (Value)v.Clone());
			return clone;
		}
	}

	[RuleEditor("Sport.Producer.UI.Rules.ScoreByPartRuleEditor, Sport.Producer.UI")]
	public class ScoreByPartRule : Sport.Rulesets.RuleType
	{
		public ScoreByPartRule(int id)
			: base(id, "ניקוד משחק לפי מערכות", Sport.Types.SportType.Match)
		{
		}

		public override Type GetDataType()
		{
			return typeof(ScoreByParts);
		}


		public override string ValueToString(object value)
		{
			ScoreByParts sbp = value as ScoreByParts;
			if (sbp != null)
			{
				return string.Join("\n", sbp.Values.ToList().ConvertAll(v => string.Format("{0}-{1}#{2}-{3}", 
					v.TeamA_PartScore, v.TeamB_PartScore, v.TeamA_GameScore, v.TeamB_GameScore)));
			}
			return null;
		}

		public override object ParseValue(string value)
		{
			if (value == null)
				return null;

			List<ScoreByParts.Value> items = new List<ScoreByParts.Value>();
			ScoreByParts.Value currentItem;
			string errorMsg;
			value.Split('\n').ToList().ForEach(rawValue =>
			{
				string[] parts = rawValue.Split('#');
				if (parts.Length == 2 && parts[0].Length > 0 && parts[1].Length > 0)
				{
					if (ScoreByParts.Value.TryCreate(parts[0], parts[1], out currentItem, out errorMsg))
					{
						items.Add(currentItem);
					}
				}
			});
			
			if (items.Count == 0)
				return null;

			ScoreByParts sbp = new ScoreByParts();
			items.ForEach(x => sbp.AddValue(x));
			return sbp;
		}

		public override void OnValueChange(Sport.Rulesets.Rule rule,
			Sport.Rulesets.RuleScope scope)
		{
		}
	}
}
