using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sport.Rulesets.Rules
{
	public class PartScore
	{
		private string[] captions = new string[] { "ניצחון", "תיקו", "הפסד" };

		public int Win { get; set; }
		public int Draw { get; set; }
		public int Lose { get; set; }

		public PartScore(int win, int draw, int lose)
		{
			this.Win = win;
			this.Draw = draw;
			this.Lose = lose;
		}

		public PartScore()
			: this(0, 0, 0)
		{
		}

		private int MapCaption(string caption)
		{
			switch (Array.IndexOf<string>(captions, caption))
			{
				case 0:
					return this.Win;
				case 1:
					return this.Draw;
				case 2:
					return this.Lose;
			}
			return 0;
		}

		public override string ToString()
		{
			return string.Join("\n", captions.ToList().ConvertAll(caption => 
				string.Format("{0}: {1}", caption, MapCaption(caption))));
		}
	}

	[RuleEditor("Sport.Producer.UI.Rules.PartScoreRuleEditor, Sport.Producer.UI")]
	public class PartScoreRule : Sport.Rulesets.RuleType
	{
		public PartScoreRule(int id)
			: base(id, "ניקוד מערכות", Sport.Types.SportType.Match)
		{
		}

		public override Type GetDataType()
		{
			return typeof(PartScore);
		}


		public override string ValueToString(object value)
		{
			PartScore ps = value as PartScore;
			if (ps != null)
				return string.Join("#", ps.Win, ps.Draw, ps.Lose);
			
			return null;
		}

		public override object ParseValue(string value)
		{
			if (value == null)
				return null;

			string[] parts = value.Split(new char[] { '#' });
			if (parts.Length < 3)
				return null;

			int win, draw, lose;
			if (!Int32.TryParse(parts[0], out win) || !Int32.TryParse(parts[1], out draw) || !Int32.TryParse(parts[2], out lose))
				return null;

			return new PartScore(win, draw, lose);
		}

		public override void OnValueChange(Sport.Rulesets.Rule rule,
			Sport.Rulesets.RuleScope scope)
		{
		}
	}
}
