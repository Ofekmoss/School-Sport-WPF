using System;
using System.Collections;

namespace Sport.Rulesets.Rules
{
	#region ScoreTable Value Class

	public class ScoreTable
	{
		private Sport.Core.Data.ResultDirection _direction;

		public Sport.Core.Data.ResultDirection Direction
		{
			get { return _direction; }
		}

		private int[] _scores;

		public int GetResult(int score)
		{
			if (_scores == null)
				throw new RulesetException("Score Table rule has not been initialized");

			if (_scores.Length == 0)
				return 0;

			if (score < 1 || (score - 1) >= _scores.Length)
				throw new RulesetException("Score out of range! (0 - " + (_scores.Length - 1) + ")");

			return _scores[score - 1];
		}

		public int GetScore(int result)
		{
			//got any result?
			if (_scores == null || result < 0)
				return 0;

			//what is the direction?
			if (_direction == Sport.Core.Data.ResultDirection.Most)
			{
				for (int n = _scores.Length - 1; n >= 0; n--)
				{
					int score = _scores[n];
					if (score > 0 && score <= result)
						return n + 1;
				}
			}
			else
			{
				for (int n = _scores.Length - 1; n >= 0; n--)
					if (_scores[n] >= result)
						return n + 1;
			}

			//out of score table...
			return 0;
		}

		public int Count
		{
			get { return (_scores == null) ? 0 : _scores.Length; }
		}

		public ScoreTable(int[] results, Sport.Core.Data.ResultDirection direction)
		{
			_direction = direction;

			_scores = (results == null) ? null : (int[])results.Clone();
		}

		public void UpdateCapacity(int capacity)
		{
			int[] scores = new int[capacity];
			if (_scores != null)
			{
				for (int n = _scores.Length - 1; n >= 0; n--)
					if (n < scores.Length)
						scores[n] = _scores[n];
			}
			_scores = scores;
		}

		public override string ToString()
		{
			return "טבלת ניקוד";
		}

		private object _tag = null;
		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}
	}

	#endregion

	[RuleEditor("Sport.Producer.UI.Rules.ScoreTableRuleEditor, Sport.Producer.UI")]
	public class ScoreTableRule : Sport.Rulesets.RuleType
	{
		public ScoreTableRule(int id)
			: base(id, "ניקוד תוצאות", Sport.Types.SportType.Competition)
		{
		}

		public override Type GetDataType()
		{
			return typeof(ScoreTable);
		}


		public override string ValueToString(object value)
		{
			ScoreTable st = value as ScoreTable;

			if (st != null)
			{
				string result = ((int)st.Direction).ToString() + '-';
				for (int n = 1; n < st.Count; n++)
					result += st.GetResult(n).ToString() + '-';
				result += st.GetResult(st.Count).ToString();
				return result;
			}

			return null;
		}

		public override object ParseValue(string value)
		{
			if (value == null)
				return null;

			string[] strings = value.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
			if (strings.Length == 0)
				return null;

			int[] results = new int[strings.Length - 1];

			for (int n = 0; n < results.Length; n++)
			{
				int curResult;
				if (Int32.TryParse(strings[n + 1], out curResult))
					results[n] = curResult;
				else
					results[n] = 0;
			}

			int direction;
			if (!Int32.TryParse(strings[0], out direction))
				direction = 0;
			return new ScoreTable(results, (Sport.Core.Data.ResultDirection)direction);
		}

		public override void OnValueChange(Sport.Rulesets.Rule rule,
			Sport.Rulesets.RuleScope scope)
		{
		}
	}
}
