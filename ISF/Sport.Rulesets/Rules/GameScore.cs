using System;

namespace Sport.Rulesets.Rules
{
	#region GameScore Value Class

	public enum GameScoreType
	{
		Win,
		Lose,
		Tie,
		TechnicalLose,
		TechnicalWin,
		ScoreEqualsPoints,
		VolleyballShortGame
	}

	public class GameScore
	{
		private double[] _scores;

		public double this[GameScoreType type]
		{
			get { return _scores[(int) type]; }
			set { _scores[(int) type] = value; }
		}

		public double Win
		{
			get { return _scores[(int) GameScoreType.Win]; }
			set { _scores[(int) GameScoreType.Win] = value; }
		}
		public double Lose
		{
			get { return _scores[(int) GameScoreType.Lose]; }
			set { _scores[(int) GameScoreType.Lose] = value; }
		}
		public double Tie
		{
			get { return _scores[(int) GameScoreType.Tie]; }
			set { _scores[(int) GameScoreType.Tie] = value; }
		}
		public double TechnicalLose
		{
			get { return _scores[(int) GameScoreType.TechnicalLose]; }
			set { _scores[(int) GameScoreType.TechnicalLose] = value; }
		}
		public double TechnicalWin
		{
			get { return _scores[(int) GameScoreType.TechnicalWin]; }
			set { _scores[(int) GameScoreType.TechnicalWin] = value; }
		}
		public bool ScoreEqualsPoints
		{
			get { return (_scores[(int) GameScoreType.ScoreEqualsPoints] == 1); }
			set { _scores[(int) GameScoreType.ScoreEqualsPoints] = ((value) ? 1 : 0); }
		}

		public bool VolleyballShortGame
		{
			get { return (_scores[(int) GameScoreType.VolleyballShortGame] == 1); }
			set { _scores[(int) GameScoreType.VolleyballShortGame] = ((value) ? 1 : 0); }
		}

		public GameScore()
		{
			_scores = new double[7];
		}

		public override string ToString()
		{
			if (this.ScoreEqualsPoints)
				return "ניקוד שווה למערכות";

			if (this.VolleyballShortGame)
				return "כדורעף מקוצר";
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			for (int n = 0; n < Data.ScoreNames.Length; n++)
			{
				double curScore = _scores[n];
				if (curScore != 0)
					sb.Append(Data.ScoreNames[n] + ": " + curScore.ToString() + "\n");
			}
			
			return sb.ToString();
		}
	}

	#endregion

	[RuleEditor("Sport.Producer.UI.Rules.GameScoreRuleEditor, Sport.Producer.UI")]
	public class GameScoreRule : Sport.Rulesets.RuleType
	{
		public GameScoreRule(int id)
			: base(id, "ניקוד משחק", Sport.Types.SportType.Match)
		{
		}
		
		public override Type GetDataType()
		{
			return typeof(GameScore);
		}
		
		public override string ValueToString(object value)
		{
			GameScore gs = value as GameScore;

			if (gs != null)
			{
				return gs.Win.ToString() + Sport.Core.Data.ScoreSeperator +
					gs.Lose.ToString() + Sport.Core.Data.ScoreSeperator +
					gs.Tie.ToString() + Sport.Core.Data.ScoreSeperator +
					gs.TechnicalLose.ToString() + Sport.Core.Data.ScoreSeperator +
					gs.TechnicalWin.ToString() + Sport.Core.Data.ScoreSeperator + 
					gs.ScoreEqualsPoints.ToString() + Sport.Core.Data.ScoreSeperator + 
					gs.VolleyballShortGame.ToString();
			}

			return null;
		}

		public override object ParseValue(string value)
		{
			if (value != null)
			{
				string[] s = value.Split(Sport.Core.Data.ScoreSeperator);
				if (s.Length < 5)
					return null;

				try
				{
					GameScore gs = new GameScore();
					gs.Win = Double.Parse(s[0]);
					gs.Lose = Double.Parse(s[1]);
					gs.Tie = Double.Parse(s[2]);
					gs.TechnicalLose = Double.Parse(s[3]);
					gs.TechnicalWin = Double.Parse(s[4]);

					string sTemp = (s.Length > 5) ? s[5] : null;
					gs.ScoreEqualsPoints = (sTemp == "1" || sTemp == "True");

					sTemp = (s.Length > 6) ? s[6] : null;
					gs.VolleyballShortGame = (sTemp == "1" || sTemp == "True");

					return gs;
				}
				catch
				{
				}
			}

			return null;
		}

		public override void OnValueChange(Sport.Rulesets.Rule rule, 
			Sport.Rulesets.RuleScope scope)
		{
		}
	}
}
