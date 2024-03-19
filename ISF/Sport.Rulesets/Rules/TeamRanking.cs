using System;

namespace Sport.Rulesets.Rules
{
	public enum RankingMethodValue
	{
		Score,					// Highest game score
		PointRatio,				// Largest scored/conceded points ratio
		PointDifference,		// Largest scored/conceded points difference
		MostPoints,				// Highest scored points
		MostSmallPoints			// Highest conceded points
	}

	public class RankingMethod
	{
		private RankingMethodValue _methodValue;
		public RankingMethodValue MethodValue
		{
			get { return _methodValue; }
			set { _methodValue = value; }
		}

		private bool _matchedTeams;
		public bool MatchedTeams
		{
			get { return _matchedTeams; }
			set { _matchedTeams = value; }
		}

		private string _pointsName = "";
		public RankingMethod(RankingMethodValue methodValue, bool matchedTeams)
		{
			_methodValue = methodValue;
			_matchedTeams = matchedTeams;
			_pointsName = "נקודות";
		}

		public RankingMethod(RankingMethodValue methodValue, bool matchedTeams,
			string pointsName)
		{
			_methodValue = methodValue;
			_matchedTeams = matchedTeams;
			if ((pointsName != null) && (pointsName.Length > 0))
			{
				_pointsName = pointsName;
			}
		}

		public override string ToString()
		{
			string s = Data.MethodValueDescription[(int)_methodValue];

			string strPointsName = _pointsName;
			s = s.Replace("[%pn]", strPointsName);
			if (s.IndexOf("זכות") >= 0)
			{
				s = s.Replace("ים", "י");
			}

			if (_matchedTeams)
				s += " (" + Data.MatchedTeamsDescription + ")";

			return s;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			return ((RankingMethod)obj).MethodValue == _methodValue &&
				((RankingMethod)obj).MatchedTeams == _matchedTeams;
		}

		public override int GetHashCode()
		{
			return (int)_methodValue + (_matchedTeams ? Int32.MinValue : 0);
		}

	}

	public class TeamRanking : ICloneable
	{
		#region RankingMethodCollection

		public class RankingMethodCollection : Sport.Common.GeneralCollection
		{
			public RankingMethod this[int index]
			{
				get { return (RankingMethod)GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, RankingMethod value)
			{
				InsertItem(index, value);
			}

			public void Remove(RankingMethod value)
			{
				RemoveItem(value);
			}

			public bool Contains(RankingMethod value)
			{
				return base.Contains(value);
			}

			public int IndexOf(RankingMethod value)
			{
				return base.IndexOf(value);
			}

			public int Add(RankingMethod value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private RankingMethodCollection rankingMethods;
		public RankingMethodCollection RankingMethods
		{
			get { return rankingMethods; }
		}

		public TeamRanking()
		{
			rankingMethods = new RankingMethodCollection();
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			for (int n = 0; n < rankingMethods.Count; n++)
			{
				sb.Append(rankingMethods[n].ToString() + "\n");
			}

			return sb.ToString();
		}

		#region ICloneable Members

		public object Clone()
		{
			TeamRanking tb = new TeamRanking();

			foreach (RankingMethod rankingMethod in rankingMethods)
			{
				tb.RankingMethods.Add(rankingMethod);
			}

			return tb;
		}

		#endregion
	}

	[RuleEditor("Sport.Producer.UI.Rules.TeamRankingRuleEditor, Sport.Producer.UI")]
	public class TeamRankingRule : Sport.Rulesets.RuleType
	{
		public TeamRankingRule(int id)
			: base(id, "דירוג קבוצות", Sport.Types.SportType.Both)
		{
		}

		public override Type GetDataType()
		{
			return typeof(TeamRanking);
		}


		public override string ValueToString(object value)
		{
			TeamRanking tb = value as TeamRanking;

			if (tb != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				for (int n = 0; n < tb.RankingMethods.Count; n++)
				{
					if (sb.Length > 0)
						sb.Append('.');
					RankingMethod rm = tb.RankingMethods[n];
					if (rm.MatchedTeams)
						sb.Append('M');
					sb.Append(((int)rm.MethodValue).ToString());
				}

				return sb.ToString();
			}

			return null;
		}

		public override object ParseValue(string value, string pointsName)
		{
			try
			{
				string[] s = value.Split(new char[] { '.' });

				TeamRanking tb = new TeamRanking();
				bool mt;
				RankingMethodValue mv;

				for (int n = 0; n < s.Length; n++)
				{
					if (s[n][0] == 'M')
					{
						mt = true;
						mv = (RankingMethodValue)Int32.Parse(s[n].Substring(1));
					}
					else
					{
						mt = false;
						mv = (RankingMethodValue)Int32.Parse(s[n]);
					}

					tb.RankingMethods.Add(new RankingMethod(mv, mt, pointsName));
				}

				return tb;
			}
			catch
			{
			}

			return null;
		}

		public override object ParseValue(string value)
		{
			return ParseValue(value, "");
		}

		public override void OnValueChange(Sport.Rulesets.Rule rule,
			Sport.Rulesets.RuleScope scope)
		{
		}
	}
}
