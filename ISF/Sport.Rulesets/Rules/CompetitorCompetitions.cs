using System;

namespace Sport.Rulesets.Rules
{
	#region CompetitorCompetitions Value Class

	public class CompetitorCompetitions
	{
		private int _minimum;
		public int Minimum
		{
			get { return _minimum; }
			set 
			{ 
				if (value > _maximum || value < 0)
					throw new ArgumentOutOfRangeException("value", "Minimum competitions amount is between 0 and maximum value");

				_minimum = value; 
			}
		}

		private int _maximum;
		public int Maximum
		{
			get { return _maximum; }
			set 
			{ 
				if (value < _minimum || value > 100)
					throw new ArgumentOutOfRangeException("value", "Maximum competitions amount is between minimum value and 10000");
				_maximum = value; 
			}
		}

		public CompetitorCompetitions(int minimum, int maximum)
		{
			Maximum = maximum;
			Minimum = minimum;
		}

		public CompetitorCompetitions(int amount)
			: this(amount, amount)
		{
		}

		public CompetitorCompetitions()
			: this(0, 0)
		{
		}

		public override string ToString()
		{
			if (_minimum == _maximum)
				return _minimum.ToString();

			return _minimum.ToString() + "-" + _maximum.ToString();
		}

	}

	#endregion

	[RuleEditor("Sport.Producer.UI.Rules.CompetitorCompetitionsRuleEditor, Sport.Producer.UI")]
	public class CompetitorCompetitionsRule : Sport.Rulesets.RuleType
	{
		public CompetitorCompetitionsRule(int id)
			: base(id, "מקצועות למשתתף", Sport.Types.SportType.Competition)
		{
		}
		
		public override Type GetDataType()
		{
			return typeof(CompetitorCompetitions);
		}


		public override string ValueToString(object value)
		{
			if (value is CompetitorCompetitions)
				return ((CompetitorCompetitions) value).ToString();
			return null;
		}

		public override object ParseValue(string value)
		{
			try
			{
				string[] s = value.Split(new char[] { '-' });
				if (s.Length == 1)
				{
					return new CompetitorCompetitions(Int32.Parse(s[0]));
				}
				else if (s.Length == 2)
				{
					return new CompetitorCompetitions(Int32.Parse(s[0]), Int32.Parse(s[1]));
				}
			}
			catch
			{
			}

			return null;
		}

		public override void OnValueChange(Sport.Rulesets.Rule rule, 
			Sport.Rulesets.RuleScope scope)
		{
		}

	}
}
