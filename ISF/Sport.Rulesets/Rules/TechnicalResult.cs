using System;
using System.Linq;
using System.Collections.Generic;

namespace Sport.Rulesets.Rules
{
	#region TechnicalResult Value Class

	public class TechnicalResult
	{
		private int _winner;
		public int Winner
		{ 
			get { return _winner; }
			set { _winner = value; }
		}

		private int _loser;
		public int Loser
		{
			get { return _loser; }
			set { _loser = value; }
		}

		private int[] _smallPoints = null;
		public int[] SmallPoints
		{
			get { return _smallPoints; }
			set { _smallPoints = value; }
		}
		
		public TechnicalResult(int winner, int loser, int[] smallPoints)
		{
			_winner = winner;
			_loser = loser;
			_smallPoints = smallPoints;
		}

		public TechnicalResult()
			: this(0, 0, null)
		{
		}

		public override string ToString()
		{
			return "מנצח: " + _winner.ToString() + "\nמפסיד: " + _loser.ToString() + "\nניקוד מערכות: " + ((_smallPoints == null) ? "" : string.Join(",", _smallPoints));
		}

	}

	#endregion

	[RuleEditor("Sport.Producer.UI.Rules.TechnicalResultRuleEditor, Sport.Producer.UI")]
	public class TechnicalResultRule : Sport.Rulesets.RuleType
	{
		public TechnicalResultRule(int id)
			: base(id, "תוצאת טכני", Sport.Types.SportType.Match)
		{
		}

		public override Type GetDataType()
		{
			return typeof(TechnicalResult);
		}


		public override string ValueToString(object value)
		{
			TechnicalResult tr = value as TechnicalResult;

			if (tr != null)
			{
				int[] smallPoints = (tr.SmallPoints == null) ? new int[] { } : tr.SmallPoints;
				return tr.Winner.ToString() + "-" + tr.Loser.ToString() + "-" + string.Join(",", smallPoints);
			}

			return null;
		}

		public override object ParseValue(string value)
		{
			if (value != null)
			{
				string[] s = value.Split(new char[] {'-'});
				if (s.Length < 2)
					return null;

				try
				{
					int winner = Int32.Parse(s[0]);
					int loser = Int32.Parse(s[1]);
					List<int> smallPoints = new List<int>();
					if (s.Length > 2)
					{
						smallPoints = s[2].Split(',').ToList().ConvertAll(p =>
							{
								int n;
								if (!Int32.TryParse(p, out n))
									n = -1;
								return n;
							}).FindAll(n => n >= 0);
					}
					return new TechnicalResult(winner, loser, smallPoints.ToArray());
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
