using System;

namespace Sport.Rulesets.Rules
{
	#region PlayersAge Value Class

	public class PlayersAge
	{
		private int _minimum;
		public int Minimum
		{
			get { return _minimum; }
			set 
			{ 
				if (value > _maximum || value < 0)
					throw new ArgumentOutOfRangeException("value", "Minimum player amount is between 0 and maximum value");

				_minimum = value; 
			}
		}

		private int _maximum;
		public int Maximum
		{
			get { return _maximum; }
			set 
			{ 
				if (value < _minimum || value > 10000)
					throw new ArgumentOutOfRangeException("value", "Maximum players amount is between minimum value and 10000");
				_maximum = value; 
			}
		}

		public PlayersAge(int minimum, int maximum)
		{
			Maximum = maximum;
			Minimum = minimum;
		}

		public PlayersAge(int amount)
			: this(amount, amount)
		{
		}

		public PlayersAge()
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

	public class PlayersAgeRule : Sport.Rulesets.RuleType
	{
		public PlayersAgeRule(int id)
			: base(id, "גיל שחקנים", Sport.Types.SportType.Both)
		{
		}
		
		public override Type GetDataType()
		{
			return typeof(PlayersAge);
		}


		public override string ValueToString(object value)
		{
			return ((PlayersAge) value).ToString();
		}

		public override object ParseValue(string value)
		{
			try
			{
				string[] s = value.Split(new char[] { '-' });
				if (s.Length == 1)
				{
					return new PlayersAge(Int32.Parse(s[0]));
				}
				else if (s.Length == 2)
				{
					return new PlayersAge(Int32.Parse(s[0]), Int32.Parse(s[1]));
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
