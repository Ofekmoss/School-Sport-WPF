using System;

namespace Sport.Rulesets.Rules
{
	#region TeamScoreCounters Value Class

	public class TeamScoreCounters : ICloneable
	{

		#region Counter Class

		public class Counter : ICloneable
		{
			private string _name;
			public string Name
			{
				get { return _name; }
				set
				{
					if (value == null)
						throw new ArgumentNullException("value", "Counter name must have a value");

					_name = value;
				}
			}

			private int _results;
			public int	Results
			{
				get { return _results; }
				set
				{
					if (value <= 0)
						throw new ArgumentOutOfRangeException("value", "Results amount must be greater than 0");

					_results = value; 
				}
			}

			public Counter(string name, int results)
			{
				Name = name;
				Results = results;
			}

			public override string ToString()
			{
				return _name + " - " + _results.ToString();
			}

			#region ICloneable Members

			public object Clone()
			{
				return new Counter(_name, _results);
			}

			#endregion
		}

		#endregion

		#region CounterCollection

		public class CounterCollection : Sport.Common.GeneralCollection
		{
			public Counter this[int index]
			{
				get { return (Counter) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Counter value)
			{
				InsertItem(index, value);
			}

			public void Remove(Counter value)
			{
				RemoveItem(value);
			}

			public bool Contains(Counter value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Counter value)
			{
				return base.IndexOf(value);
			}

			public int Add(Counter value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private CounterCollection counters;
		public CounterCollection Counters
		{
			get { return counters; }
		}

		public TeamScoreCounters()
		{
			counters = new CounterCollection();
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach (Counter counter in counters)
			{
				sb.Append(counter.ToString());
				sb.Append('\n');
			}

			return sb.ToString();
		}

		#region ICloneable Members

		public object Clone()
		{
			TeamScoreCounters tsc = new TeamScoreCounters();

			foreach (Counter counter in counters)
			{
				tsc.Counters.Add((Counter) counter.Clone());
			}

			return tsc;
		}

		#endregion
	}

	#endregion

	[RuleEditor("Sport.Producer.UI.Rules.TeamScoreCountersRuleEditor, Sport.Producer.UI")]
	public class TeamScoreCountersRule : Sport.Rulesets.RuleType
	{
		public TeamScoreCountersRule(int id)
			: base(id, "מוני תוצאה", Sport.Types.SportType.Competition)
		{
		}
		
		public override Type GetDataType()
		{
			return typeof(TeamScoreCounters);
		}


		public override string ValueToString(object value)
		{
			TeamScoreCounters tsc = value as TeamScoreCounters;

			if (tsc != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				foreach (TeamScoreCounters.Counter counter in tsc.Counters)
				{
					sb.Append(counter.Name);
					sb.Append('\n');
					sb.Append(counter.Results);
					sb.Append('\n');
				}

				return sb.ToString();
			}

			return null;
		}

		public override object ParseValue(string value)
		{
			try
			{
				string[] lines = value.Split(new char[] { '\n' });

				if (lines.Length > 1)
				{
					TeamScoreCounters tsc = new TeamScoreCounters();

					int n = 0;
					while (n + 1 < lines.Length)
					{
						tsc.Counters.Add(new TeamScoreCounters.Counter(lines[n], Int32.Parse(lines[n + 1])));
						n += 2;
					}

					return tsc;
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
