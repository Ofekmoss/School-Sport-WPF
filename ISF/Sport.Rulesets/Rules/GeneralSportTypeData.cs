using System;

namespace Sport.Rulesets.Rules
{
	#region GeneralSportTypeData Value Class
	public class GeneralSportTypeData
	{
		private bool _hasLanes = true;
		public bool HasLanes
		{
			get { return _hasLanes; }
			set { _hasLanes = value; }
		}

		private bool _scoreIsRank = false;
		public bool ScoreIsRank
		{
			get { return _scoreIsRank; }
			set { _scoreIsRank = value; }
		}

		private int _tries = 1;
		public int Tries
		{
			get { return _tries; }
			set
			{
				_tries = value;
				if (_tries < 1)
					_tries = 1;
			}
		}

		private int _results = 1;
		public int Results
		{
			get { return _results; }
			set
			{
				_results = value;
				if (_results < 1)
					_results = 1;
			}
		}

		private bool _doubleScore = false;
		public bool DoubleScore
		{
			get { return _doubleScore; }
			set { _doubleScore = value; }
		}

		private bool _sharedResults = false;
		public bool SharedResults
		{
			get { return _sharedResults; }
			set { _sharedResults = value; }
		}

		private bool _disqualifications = false;
		public bool Disqualifications
		{
			get { return _disqualifications; }
			set { _disqualifications = value; }
		}

		private bool _wind = false;
		public bool Wind
		{
			get { return _wind; }
			set { _wind = value; }
		}

		public GeneralSportTypeData(bool hasLanes, int tries, int results, bool scoreIsRank, 
			bool doubleScore, bool sharedResults, bool disqualifications, bool wind)
		{
			HasLanes = hasLanes;
			Tries = tries;
			Results = results;
			ScoreIsRank = scoreIsRank;
			DoubleScore = doubleScore;
			SharedResults = sharedResults;
			Disqualifications = disqualifications;
			Wind = wind;
		}

		public GeneralSportTypeData(bool hasLanes)
			: this(hasLanes, 1, 1, false, false, false, false, false)
		{
		}

		public GeneralSportTypeData()
			: this(true)
		{
		}

		public override string ToString()
		{
			string result = "";
			if (HasLanes == false)
				result += "אין מסלולים";
			if (ScoreIsRank == true)
			{
				if (result.Length > 0)
					result += "\n";
				result += "ניקוד שווה לדירוג";
			}
			if (Tries > 1)
			{
				if (result.Length > 0)
					result += "\n";
				result += "מספר נסיונות: " + Tries.ToString();
			}
			if (Results > 1)
			{
				if (result.Length > 0)
					result += "\n";
				result += "מספר תוצאות: " + Results.ToString();
			}
			if (DoubleScore == true)
			{
				if (result.Length > 0)
					result += "\n";
				result += "ניקוד כפול";
			}
			if (SharedResults == true)
			{
				if (result.Length > 0)
					result += "\n";
				result += "תוצאות משותפות";
			}
			if (Disqualifications == true)
			{
				if (result.Length > 0)
					result += "\n";
				result += "פסילות";
			}
			if (Wind == true)
			{
				if (result.Length > 0)
					result += "\n";
				result += "רוח";
			}
			return (result.Length == 0) ? "הגדרות ברירת מחדל" : result;
		}
	}
	#endregion

	[RuleEditor("Sport.Producer.UI.Rules.GeneralSportTypeDataRuleEditor, Sport.Producer.UI")]
	public class GeneralSportTypeDataRule : Sport.Rulesets.RuleType
	{
		public GeneralSportTypeDataRule(int id)
			: base(id, "הגדרות כלליות", Sport.Types.SportType.Competition)
		{
		}

		public override Type GetDataType()
		{
			return typeof(GeneralSportTypeData);
		}

		public override string ValueToString(object value)
		{
			string result = "";
			GeneralSportTypeData data = (GeneralSportTypeData)value;
			result += data.HasLanes ? "1" : "0";
			result += "-";
			result += data.Tries.ToString();
			result += "-";
			result += data.Results.ToString();
			result += "-";
			result += data.ScoreIsRank ? "1" : "0";
			result += "-";
			result += data.DoubleScore ? "1" : "0";
			result += "-";
			result += data.SharedResults ? "1" : "0";
			result += "-";
			result += data.Disqualifications ? "1" : "0";
			result += "-";
			result += data.Wind ? "1" : "0";
			return result;
		}

		public override object ParseValue(string value)
		{
			if ((value == null) || (value.Length == 0))
				return null;
			string[] arrTemp = value.Split(new char[] { '-' });
			bool blnHasLanes = (arrTemp[0] == "1");
			int tries = (arrTemp.Length > 1) ? Int32.Parse(arrTemp[1]) : 1;
			int results = (arrTemp.Length > 2) ? Int32.Parse(arrTemp[2]) : 1;
			bool blnScoreIsRank = ((arrTemp.Length > 3) && (arrTemp[3] == "1"));
			bool blnDoubleScore = ((arrTemp.Length > 4) && (arrTemp[4] == "1"));
			bool blnSharedResults = ((arrTemp.Length > 5) && (arrTemp[5] == "1"));
			bool blnDisqualifications = ((arrTemp.Length > 6) && (arrTemp[6] == "1"));
			bool blnWind = ((arrTemp.Length > 7) && (arrTemp[7] == "1"));
			return new GeneralSportTypeData(blnHasLanes, tries, results, blnScoreIsRank, blnDoubleScore, blnSharedResults, blnDisqualifications, blnWind);
		}

		public override void OnValueChange(Sport.Rulesets.Rule rule,
			Sport.Rulesets.RuleScope scope)
		{
		}
	}
}
