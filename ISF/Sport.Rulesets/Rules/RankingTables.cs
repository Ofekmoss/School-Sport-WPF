using System;

namespace Sport.Rulesets.Rules
{
	public enum TeamRankField
	{
		Position,			// R - rank
		Score,				// S - score
		Points,				// P - points
		PointsAgainst,		// C - conceded points
		SmallPoints,		// T - small points
		SmallPointsAgainst,	// Y - conceded small points
		Games,				// G - games
		Wins,				// W - wins
		Loses,				// L - loses
		TechnicalWins,		// E - techWins
		TechnicalLoses,		// F - techLoses
		Ties				// D - ties
	}

	// Match team rank fields
	// R - rank
	// S - score
	// G - games
	// P - points
	// C - conceded points
	// T - small points
	// Y - conceded small points
	// W - wins
	// L - loses
	// E - techWins
	// F - techLoses
	// D - ties

	// Competition team rank fields
	// R - rank
	// S - score
	// C - counted results
	// Cn - counted results for counter n
	// Sn - score for counter n
	//			In counters scoring plan the first counters are as defined by rule 
	//			and the last is the additional results counter
	//			In multi challenge scoring plan the first counters are the highest scored players
	//			and the last is the additional results counter

	public class RankField : ICloneable
	{
		private string _title;
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		private string _value;
		public string Value
		{
			get { return _value; }
			set 
			{ 
				_value = value; 
				EvaluateEquations();
			}
		}

		private string					_evaluated;
		private Sport.Common.Equation[] _equations;
		
		private void EvaluateEquations()
		{
			System.Collections.ArrayList al = new System.Collections.ArrayList();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
			int start = 0;
			int end;
			int pos = _value.IndexOf('{');
			while (pos >= 0)
			{
				if (pos > 0)
					sb.Append(_value.Substring(start, pos - start));
				end = _value.IndexOf('}', pos + 1);

				if (end < 0)
				{
					pos = -1;
				}
				else
				{
					string s = _value.Substring(pos + 1, end - pos - 1);
					pos = s.IndexOf(':');
					string format;
					if (pos >= 0)
					{
						format = s.Substring(pos);
						s = s.Substring(0, pos);
					}
					else
					{
						format = ":######0.###";
					}

					try
					{
						Sport.Common.Equation eq = Sport.Common.Equation.Evaluate(s);
						sb.Append("{" + al.Add(eq).ToString() + format + "}");
					}
					catch
					{
						sb.Append(_value.Substring(pos, end - pos + 1));
					}

					start = end + 1;
					pos = _value.IndexOf('{', start);
				}
			}

			sb.Append(_value.Substring(start));

			_evaluated = sb.ToString();
			_equations = (Sport.Common.Equation[]) al.ToArray(typeof(Sport.Common.Equation));
		}

		public string Evaluate(Sport.Common.EquationVariables variables)
		{
			if (_equations.Length == 0)
				return _evaluated;

			object[] values = new object[_equations.Length];

			for (int n = 0; n < _equations.Length; n++)
			{
				try
				{
					values[n] = _equations[n].Resolve(variables);
				}
				catch //(Exception ex)
				{
					//System.Diagnostics.Debug.WriteLine("error evaluating variables. index: " + n + ", message: " + ex.Message + ", stack: " + ex.StackTrace);
					values[n] = "???";
				}
			}

			return String.Format(_evaluated, values);
		}

		public RankField(string title, string value)
		{
			_title = title;
			_value = value;
			EvaluateEquations();
		}

		public override string ToString()
		{
			return _title + " (" + _value + ")";
		}

		#region ICloneable Members

		public object Clone()
		{
			return new RankField(_title, _value);
		}

		#endregion
	}

	public class RankingTable : ICloneable
	{
		#region RankFieldCollection

		public class RankFieldCollection : Sport.Common.GeneralCollection
		{
			public RankField this[int index]
			{
				get { return (RankField) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, RankField value)
			{
				InsertItem(index, value);
			}

			public void Remove(RankField value)
			{
				RemoveItem(value);
			}

			public bool Contains(RankField value)
			{
				return base.Contains(value);
			}

			public int IndexOf(RankField value)
			{
				return base.IndexOf(value);
			}

			public int Add(RankField value)
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

		private RankFieldCollection fields;
		public RankFieldCollection Fields
		{
			get { return fields; }
		}

		public RankingTable()
		{
			fields = new RankFieldCollection();
		}

		public override string ToString()
		{
			return _name;
		}

		#region ICloneable Members

		public object Clone()
		{
			RankingTable rt = new RankingTable();

			rt._name = _name;

			foreach (RankField field in fields)
			{
				rt.Fields.Add((RankField) field.Clone());
			}

			return rt;
		}

		#endregion
	}

	public class RankingTables : ICloneable
	{
		#region RankingTableCollection

		public class RankingTableCollection : Sport.Common.GeneralCollection
		{
			public RankingTable this[int index]
			{
				get { return (RankingTable) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, RankingTable value)
			{
				InsertItem(index, value);
			}

			public void Remove(RankingTable value)
			{
				RemoveItem(value);
			}

			public bool Contains(RankingTable value)
			{
				return base.Contains(value);
			}

			public int IndexOf(RankingTable value)
			{
				return base.IndexOf(value);
			}

			public int Add(RankingTable value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private RankingTable _defaultRankingTable;
		public RankingTable DefaultRankingTable
		{
			get { return _defaultRankingTable; }
			set 
			{ 
				if (tables.Contains(value))
				{
					_defaultRankingTable = value;
				}
			}
		}

		private RankingTableCollection tables;
		public RankingTableCollection Tables
		{
			get { return tables; }
		}

		public RankingTables()
		{
			_defaultRankingTable = null;
			tables = new RankingTableCollection();
		}

		public override string ToString()
		{
			if (_defaultRankingTable != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				foreach (RankField field in _defaultRankingTable.Fields)
				{
					sb.Append(field.ToString());
					sb.Append('\n');
				}

				return sb.ToString();
			}

			if (tables.Count > 0)
			{
				return string.Format("{0} טבלאות", tables.Count);
			}

			return "ריק";
		}

		#region ICloneable Members

		public object Clone()
		{
			RankingTables rts = new RankingTables();

			foreach (RankingTable rankingTable in Tables)
			{
				RankingTable clonedTable = (RankingTable) rankingTable.Clone();
				rts.Tables.Add(clonedTable);
				if (rankingTable == _defaultRankingTable)
				{
					rts.DefaultRankingTable = clonedTable;
				}
			}

			return rts;
		}

		#endregion
	}

	[RuleEditor("Sport.Producer.UI.Rules.RankingTablesRuleEditor, Sport.Producer.UI")]
	public class RankingTablesRule : Sport.Rulesets.RuleType
	{
		public const string PhaseTable = "טבלת דירוג";

		public RankingTablesRule(int id)
			: base(id, "טבלאות דירוג", Sport.Types.SportType.Both)
		{
		}

		public override Type GetDataType()
		{
			return typeof(RankingTables);
		}

		private void AddRankingTable(RankingTable rankingTable, System.Text.StringBuilder sb)
		{
			foreach (RankField field in rankingTable.Fields)
			{
				sb.Append(field.Title);
				sb.Append('\n');
				sb.Append(field.Value);
				sb.Append('\n');
			}

			// Adding name after table for backward compatability
			sb.Append('\n');
			sb.Append(rankingTable.Name);
			sb.Append('\n');
		}

		public override string ValueToString(object value)
		{
			RankingTables rts = value as RankingTables;

			if (rts != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				// Starting with default ranking table
				if (rts.DefaultRankingTable != null)
				{
					AddRankingTable(rts.DefaultRankingTable, sb);
				}

				foreach (RankingTable rankingTable in rts.Tables)
				{
					// Default table was already added
					if (rankingTable != rts.DefaultRankingTable)
					{
						AddRankingTable(rankingTable, sb);
					}
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
					RankingTables rts = new RankingTables();
					RankingTable rankingTable = new RankingTable();
					RankingTable defaultRankingTable = rankingTable;

					int n = 0;
					while (n + 1 < lines.Length)
					{
						// Checking if this is a name separator
						if (lines[n].Length == 0)
						{
							rankingTable.Name = lines[n + 1];
							rts.Tables.Add(rankingTable);
							rankingTable = new RankingTable();
						}
						else
						{
							rankingTable.Fields.Add(new RankField(lines[n], lines[n + 1]));
						}
						n += 2;
					}

					// Checking if the last table had fields (maybe just didn't have
					// a name if this is from an older version)
					if (rankingTable.Fields.Count > 0)
					{
						if (rankingTable.Name == null)
						{
							rankingTable.Name = "ברירת מחדל";
						}
						rts.Tables.Add(rankingTable);
					}

					// Checking if any table was added...
					if (rts.Tables.Count > 0)
					{
						// ... setting default ranking table
						rts.DefaultRankingTable = defaultRankingTable;
					}

					return rts;
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
			return new string[] { PhaseTable };
		}

		public override string GetDefinitionDefaultValue(string definition, object ruleValue)
		{
			if (definition == PhaseTable)
			{
				RankingTables rts = ruleValue as RankingTables;
				if (rts != null && rts.DefaultRankingTable != null)
				{
					return rts.DefaultRankingTable.Name;
				}
			}

			return null;
		}

		public override string[] GetDefinitionValues(string definition, object ruleValue)
		{
			if (definition == PhaseTable)
			{
				RankingTables rts = ruleValue as RankingTables;
				if (rts != null)
				{
					int count = rts.Tables.Count;
					if (rts.DefaultRankingTable != null)
						count--;

					string[] tables = new string[count];
					int table = 0;
					for (int n = 0; n < rts.Tables.Count; n++)
					{
						if (rts.Tables[n] != rts.DefaultRankingTable)
						{
							tables[table] = rts.Tables[n].Name;
							table++;
						}
					}

					return tables;
				}
			}

			return base.GetDefinitionValues (definition, ruleValue);
		}



	}
}
