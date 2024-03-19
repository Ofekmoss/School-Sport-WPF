using System;
using Sport.Core;
using Sport.Common;

namespace Sport.Rulesets.Rules
{
	#region CompetitionResult Class
/*
	public class CompetitionResult
	{
		private int _result;
		public int Result
		{
			get { return _result; }
			set { _result = value; }
		}

		private ResultType _resultType;

		#region Distance properties

		public int Kilometers
		{
			get { return _result / 1000000; }
			set { _result -= (Kilometers - value) * 1000000; }
		}

		public int Meters
		{
			get { return _result - Kilometers * 1000000; }
			set { _result -= (Meters - value); }
		}

		public int Centimeters
		{
			get { return (int) ((_result - (int) _result) * 100); }
			set { _result -= (int) (Centimeters - value) / 100; }
		}

		public int Millimeters
		{
			get { return (int) (_result * 1000 - (((int) _result * 100) * 10)); }
			set { _result -= (int) (Millimeters - value) / 1000; }
		}

		#endregion

		#region Time properties

		public int Days
		{
			get { return (int) (_result / 86400); }
			set { _result -= (Days - value) * 86400; }
		}

		public int Hours
		{
			get { return (int) (_result - Days * 86400) / 3600; }
			set { _result -= (Hours - value) * 3600; }
		}

		public int Minutes
		{
			get { return (int) (_result - Days * 86400 - Hours * 3600) / 60; }
			set { _result -= (Minutes - value) * 60; }
		}

		public int Seconds
		{
			get { return (int) _result - Days* 86400 - Hours * 3600 - Minutes * 60; }
			set { _result -= Seconds - value; }
		}

		public int Milliseconds
		{
			get { return (int) ((_result - (int) _result) * 1000); }
			set { _result -= (int) (Milliseconds - value) / 1000; }
		}

		#endregion

		public CompetitionResult(int result, ResultType resultType)
		{
			_result = result;
			_resultType = resultType;
		}

		public override string ToString()
		{
			return _resultType.FormatResult(_result);
		}
	}
*/
	#endregion

	#region ResultType Value Class

	public class ResultType
	{
		private Sport.Core.Data.ResultValue _value;
		public Sport.Core.Data.ResultValue Value
		{
			get { return _value; }
			set { _value = value; }
		}

		private Sport.Core.Data.ResultDirection _direction;
		public Sport.Core.Data.ResultDirection Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}

		public Sport.Core.Data.ResultMeasure Measures
		{
			get { return (Sport.Core.Data.ResultMeasure) 0; }
		}

		public ResultFormat ResultFormat
		{
			get { return new ResultFormat(); }
		}

		private ValueFormatter _valueFormatter;
		public ValueFormatter ValueFormatter
		{
			get { return _valueFormatter; }
			set { _valueFormatter = value; }
		}

		public ResultType(Sport.Core.Data.ResultValue value, Sport.Core.Data.ResultDirection direction, 
			string format)
		{
			_value = value;
			_direction = direction;

			if (format != null)
			{
				switch (_value)
				{
					case (Sport.Core.Data.ResultValue.Time):
						_valueFormatter = new ValueFormatter(ValueFormatter.TimeValueFormatters, format);
						break;
					case (Sport.Core.Data.ResultValue.Distance):
						_valueFormatter = new ValueFormatter(ValueFormatter.DistanceValueFormatters, format);
						break;
					case (Sport.Core.Data.ResultValue.Points):
						_valueFormatter = new ValueFormatter(ValueFormatter.PointValueFormatters, format);
						break;
				}
			}
		}

		public ResultType(Sport.Core.Data.ResultValue value, Sport.Core.Data.ResultDirection direction)
			: this(value, direction, null)
		{
		}

		public ResultType(Sport.Core.Data.ResultValue value)
			: this(value, Sport.Core.Data.ResultDirection.Most)
		{
		}

		public string FormatResult(int result)
		{
			if (result == -1)
				return null;

			return _valueFormatter.GetText(result);
		}


		public override string ToString()
		{
			if (Data.ValueNames == null || Data.DirectionNames == null || _valueFormatter == null)
				return "";
			
			return Data.ValueNames[(int) _value] + " - " + Data.DirectionNames[(int) _direction] + 
				" (" + _valueFormatter.Format + ")";
		}
		
		public ScoreTable[] GetScoreTables()
		{
			SportServices.RulesetService service=new SportServices.RulesetService();
			string strFormat=((int) this.Value).ToString() + "-" +
					((int) this.Direction).ToString() + "-" +
					this.ValueFormatter.Format;
			System.Collections.ArrayList arrData=new System.Collections.ArrayList(
				service.GetScoreTablesByFormat(strFormat));
			if ((arrData == null)||(arrData.Count == 0))
				return null;
			System.Collections.ArrayList result=new System.Collections.ArrayList();
			foreach (string strData in arrData)
			{
				string[] data=strData.Split(new char[] {'|'});
				int rulesetID=Int32.Parse(data[0]);
				int sportFieldType=Int32.Parse(data[1]);
				int sportField=Int32.Parse(data[2]);
				int category=Int32.Parse(data[3]);
				if ((rulesetID < 0)||(sportFieldType < 0))
					continue;
				if (category <= 0)
					category = Sport.Types.CategoryTypeLookup.All;
				Ruleset ruleset=Ruleset.LoadRuleset(rulesetID);
				RuleScope scope=new RuleScope(category, sportFieldType);
				if (sportField >= 0)
					scope.SportField = sportField;
				ScoreTable curScoreTable=null;
				object objScoreTable = 
					ruleset.GetRule(scope, typeof(ScoreTable), false);
				if ((objScoreTable != null)&&(objScoreTable is ScoreTable))
					curScoreTable = (ScoreTable) objScoreTable;
				if (curScoreTable != null)
				{
					curScoreTable.Tag = 
						new int[] {rulesetID, sportFieldType, sportField, category};
					result.Add(curScoreTable);
				}
			}
			return (ScoreTable[]) result.ToArray(typeof(ScoreTable));
		}
	}

	#endregion

	[RuleEditor("Sport.Producer.UI.Rules.ResultTypeRuleEditor, Sport.Producer.UI")]
	public class ResultTypeRule : Sport.Rulesets.RuleType
	{
		public ResultTypeRule(int id)
			: base(id, "סוג תוצאה", Sport.Types.SportType.Competition)
		{
		}

		public override Type GetDataType()
		{
			return typeof(ResultType);
		}


		public override string ValueToString(object value)
		{
			if (value != null)
			{
				ResultType st = (ResultType) value;
				if (st.ValueFormatter != null)
				{
					return ((int) st.Value).ToString() + "-" +
						((int) st.Direction).ToString() + "-" +
						st.ValueFormatter.Format;
				}
			}

			return null;
		}

		public override object ParseValue(string value)
		{
			if (value == null)
				return null;

			int s1 = value.IndexOf('-');
			int s2 = value.IndexOf('-', s1 + 1);

			if (s1 >= 0 && s2 > s1)
			{
				try
				{
					Sport.Core.Data.ResultValue rv = (Sport.Core.Data.ResultValue) Int32.Parse(value.Substring(0, s1));
					Sport.Core.Data.ResultDirection rd = (Sport.Core.Data.ResultDirection) Int32.Parse(value.Substring(s1 + 1, s2 - s1 - 1));
					string format = value.Substring(s2 + 1);

					return new ResultType(rv, rd, format);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("Invalid result type rule value: "+value);
					System.Diagnostics.Debug.WriteLine(e.StackTrace);
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
