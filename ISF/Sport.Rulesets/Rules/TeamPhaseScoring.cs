using System;

namespace Sport.Rulesets.Rules
{
	public enum ScoringPlanType
	{
		Counters,
		MultiChallenge
	}

	public class ScoringPlan : ICloneable
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

		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private ScoringPlanType _planType;
		public ScoringPlanType PlanType
		{
			get { return _planType; }
			set { _planType = value; }
		}
		
		private int _additionalResults;
		public int AdditionalResults
		{
			get { return _additionalResults; }
			set { _additionalResults = value; }
		}

		private CounterCollection counters;
		public CounterCollection Counters
		{
			get { return counters; }
		}

		private int _multiChallengeResults;
		public int MultiChallengeResults
		{
			get { return _multiChallengeResults; }
			set { _multiChallengeResults = value; }
		}

		public ScoringPlan()
		{
			counters = new CounterCollection();
		}

		public override string ToString()
		{
			return _name;
		}
	
		#region ICloneable Members

		public object Clone()
		{
			ScoringPlan sp = new ScoringPlan();

			sp._name = _name;
			sp._planType = _planType;
			sp._additionalResults = _additionalResults;
			sp._multiChallengeResults = _multiChallengeResults;

			foreach (Counter counter in counters)
			{
				sp.Counters.Add((Counter) counter.Clone());
			}

			return sp;
		}

		#endregion
	}

	/// <summary>
	/// Summary description for TeamPhaseScoring.
	/// </summary>
	public class TeamPhaseScoring : ICloneable
	{
		#region ScoringPlanCollection

		public class ScoringPlanCollection : Sport.Common.GeneralCollection
		{
			public ScoringPlan this[int index]
			{
				get { return (ScoringPlan) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, ScoringPlan value)
			{
				InsertItem(index, value);
			}

			public void Remove(ScoringPlan value)
			{
				RemoveItem(value);
			}

			public bool Contains(ScoringPlan value)
			{
				return base.Contains(value);
			}

			public int IndexOf(ScoringPlan value)
			{
				return base.IndexOf(value);
			}

			public int Add(ScoringPlan value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private ScoringPlanCollection scoringPlans;
		public ScoringPlanCollection ScoringPlans
		{
			get { return scoringPlans; }
		}

		public TeamPhaseScoring()
		{
			scoringPlans = new ScoringPlanCollection();
		}

		public ScoringPlan GetScoringPlan(string scoringPlanName)
		{
			foreach (ScoringPlan scoringPlan in scoringPlans)
			{
				if (scoringPlan.Name == scoringPlanName)
				{
					return scoringPlan;
				}
			}
			
			return null;
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			bool firstPlan = true;
			foreach (ScoringPlan scoringPlan in scoringPlans)
			{
				if (firstPlan)
				{
					firstPlan = false;
				}
				else
				{
					sb.Append('\n');
				}

				sb.Append(scoringPlan.Name);
				sb.Append(": ");

				bool firstCounter = true;

				if (scoringPlan.PlanType == ScoringPlanType.MultiChallenge)
				{
					sb.Append("קרב רב - " + scoringPlan.MultiChallengeResults.ToString() + " תוצאות גבוהות");
				}
				else
				{
					foreach (ScoringPlan.Counter counter in scoringPlan.Counters)
					{
						if (firstCounter)
						{
							firstCounter = false;
						}
						else
						{
							sb.Append(", ");
						}
						sb.Append(counter.ToString());
					}
				}

				if (scoringPlan.AdditionalResults > 0)
				{
					if (!firstCounter)
					{
						sb.Append(", ");
					}
					sb.Append("תוצאות נוספות - " + scoringPlan.AdditionalResults.ToString());
				}
			}

			return sb.ToString();
		}

		#region ICloneable Members

		public object Clone()
		{
			TeamPhaseScoring tps = new TeamPhaseScoring();

			foreach (ScoringPlan scoringPlan in scoringPlans)
			{
				tps.ScoringPlans.Add((ScoringPlan) scoringPlan.Clone());
			}

			return tps;
		}

		#endregion
	}

	[RuleEditor("Sport.Producer.UI.Rules.TeamPhaseScoringRuleEditor, Sport.Producer.UI")]
	public class TeamPhaseScoringRule : Sport.Rulesets.RuleType
	{
		public const string PhaseScoring = "ניקוד שלב";
		public TeamPhaseScoringRule(int id)
			: base(id, "ניקוד קבוצות לשלב", Sport.Types.SportType.Competition)
		{
		}
		
		public override Type GetDataType()
		{
			return typeof(TeamPhaseScoring);
		}


		public override string ValueToString(object value)
		{
			TeamPhaseScoring tps = value as TeamPhaseScoring;

			if (tps != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				foreach (ScoringPlan scoringPlan in tps.ScoringPlans)
				{
					sb.Append(scoringPlan.Name);
					sb.Append('\n');
					sb.Append(((int) scoringPlan.PlanType).ToString() + ":" + scoringPlan.AdditionalResults.ToString() + ":" + scoringPlan.MultiChallengeResults.ToString());
					sb.Append('\n');
					foreach (ScoringPlan.Counter counter in scoringPlan.Counters)
					{
						sb.Append(counter.Name);
						sb.Append('\n');
						sb.Append(counter.Results);
						sb.Append('\n');
					}
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
					TeamPhaseScoring tps = new TeamPhaseScoring();

					int n = 0;
					while (n + 2 < lines.Length)
					{
						ScoringPlan scoringPlan = new ScoringPlan();
						scoringPlan.Name = lines[n];
						string[] values = lines[n + 1].Split(new char[] { ':' });
						if (values.Length != 3)
							return null;
						scoringPlan.PlanType = (ScoringPlanType) Int32.Parse(values[0]);
						scoringPlan.AdditionalResults = Int32.Parse(values[1]);
						scoringPlan.MultiChallengeResults = Int32.Parse(values[2]);
						n += 2;

						while (lines[n].Length != 0)
						{
							scoringPlan.Counters.Add(new ScoringPlan.Counter(lines[n], Int32.Parse(lines[n + 1])));
							n += 2;
						}
						tps.ScoringPlans.Add(scoringPlan);
						n++;
					}

					return tps;
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

		public override string[] GetDefinitions()
		{
			return new string [] { PhaseScoring };
		}

		public override string[] GetDefinitionValues(string definition, object ruleValue)
		{
			TeamPhaseScoring tps = ruleValue as TeamPhaseScoring;
			if (tps != null)
			{
				string[] plans = new string[tps.ScoringPlans.Count];
				for (int n = 0; n < plans.Length; n++)
				{
					plans[n] = tps.ScoringPlans[n].Name;
				}

				return plans;
			}
			return base.GetDefinitionValues (definition, ruleValue);
		}
	}
}
