using System;

namespace Sport.Rulesets.Rules
{
	#region TeamScoreCounter Value Class

	public class TeamScoreCounter
	{
		private string _counter;
		public string Counter
		{
			get { return _counter; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value", "Result counter must have a value");

				_counter = value;
			}
		}

		public TeamScoreCounter(string counter)
		{
			Counter = counter;
		}

		public override string ToString()
		{
			return _counter;
		}

	}

	#endregion

	[RuleEditor("Sport.Producer.UI.Rules.TeamScoreCounterRuleEditor, Sport.Producer.UI")]
	public class TeamScoreCounterRule : Sport.Rulesets.RuleType
	{
		public TeamScoreCounterRule(int id)
			: base(id, "מונה תוצאה", Sport.Types.SportType.Competition)
		{
		}
		
		public override Type GetDataType()
		{
			return typeof(TeamScoreCounter);
		}


		public override string ValueToString(object value)
		{
			if (value != null)
				return (value as TeamScoreCounter).ToString();
			return null;
		}

		public override object ParseValue(string value)
		{
			try
			{
				return new TeamScoreCounter(value);
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
