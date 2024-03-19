using System;
using Sport.Rulesets.Rules;

namespace Sport.Rulesets
{
	public class Tools
	{
		public static string ParseResultType(ResultType resultType)
		{
			string result="-לא ידוע-";
			switch (resultType.Value)
			{
				case Sport.Core.Data.ResultValue.Time:
					result = ParseTimeMeasure(resultType);
					break;
				case Sport.Core.Data.ResultValue.Distance:
					result = ParseDistanceMeasure(resultType);
					break;
				case Sport.Core.Data.ResultValue.Points:
					result = "נקודות";
					break;
			}
			return result;
		}

		public static string ParseTimeMeasure(ResultType resultType)
		{
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Days) != 0)
				return "ימים";
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Hours) != 0)
				return "שעות";
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Minutes) != 0)
				return "דקות";
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Seconds) != 0)
				return "שניות";
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Miliseconds) != 0)
				return "אלפיות שניה";
			return "לא ידוע";
		}
		
		public static string ParseDistanceMeasure(ResultType resultType)
		{
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Kilometers) != 0)
				return "קילומטרים";
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Meters) != 0)
				return "מטרים";
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Centimeters) != 0)
				return "סנטימטרים";
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Milimeters) != 0)
				return "מילימטרים";
			return "לא ידוע";
		}
		
		/// <summary>
		/// returns the relative value. for example 1 minute is 60 seconds.
		/// </summary>
		public static int CalculateRelativeValue(Sport.Core.Data.ResultValue resultValue,
			Sport.Core.Data.ResultMeasure primeMeasure, Sport.Core.Data.ResultMeasure relativeMeasure, int value)
		{
			//prime measure must be "deeper" than relative measure...
			if ((int) primeMeasure < (int) relativeMeasure)
				return value;
			
			//maybe just equal?
			if ((int) primeMeasure == (int) relativeMeasure)
				return value;

			if (resultValue == Sport.Core.Data.ResultValue.Time)
			{
				//for now use nested switch.......
				switch (primeMeasure)
				{
					case Sport.Core.Data.ResultMeasure.Miliseconds:
					switch (relativeMeasure)
					{
						case Sport.Core.Data.ResultMeasure.Seconds:
							return 1000*value;
						case Sport.Core.Data.ResultMeasure.Minutes:
							return 60*CalculateRelativeValue(resultValue, 
								primeMeasure, Sport.Core.Data.ResultMeasure.Seconds, value);
						case Sport.Core.Data.ResultMeasure.Hours:
							return 60*CalculateRelativeValue(resultValue, 
								primeMeasure, Sport.Core.Data.ResultMeasure.Minutes, value);
						case Sport.Core.Data.ResultMeasure.Days:
							return 24*CalculateRelativeValue(resultValue, 
								primeMeasure, Sport.Core.Data.ResultMeasure.Hours, value);
						default:
							return value;
					}
					case Sport.Core.Data.ResultMeasure.Seconds:
					switch (relativeMeasure)
					{
						case Sport.Core.Data.ResultMeasure.Minutes:
							return 60*value;
						case Sport.Core.Data.ResultMeasure.Hours:
							return 60*CalculateRelativeValue(resultValue, 
								primeMeasure, Sport.Core.Data.ResultMeasure.Minutes, value);
						case Sport.Core.Data.ResultMeasure.Days:
							return 24*CalculateRelativeValue(resultValue, 
								primeMeasure, Sport.Core.Data.ResultMeasure.Hours, value);
						default:
							return value;
					}
					case Sport.Core.Data.ResultMeasure.Minutes:
					switch (relativeMeasure)
					{
						case Sport.Core.Data.ResultMeasure.Hours:
							return 60*value;
						case Sport.Core.Data.ResultMeasure.Days:
							return 24*CalculateRelativeValue(resultValue, 
								primeMeasure, Sport.Core.Data.ResultMeasure.Hours, value);
						default:
							return value;
					}
					case Sport.Core.Data.ResultMeasure.Hours:
					switch (relativeMeasure)
					{
						case Sport.Core.Data.ResultMeasure.Days:
							return 24*value;
						default:
							return value;
					}
				} //end switch prime measure...
			}
			
			if (resultValue == Sport.Core.Data.ResultValue.Distance)
			{
				//for now use nested switch.......
				switch (primeMeasure)
				{
					case Sport.Core.Data.ResultMeasure.Milimeters:
					switch (relativeMeasure)
					{
						case Sport.Core.Data.ResultMeasure.Centimeters:
							return 10*value;
						case Sport.Core.Data.ResultMeasure.Meters:
							return 100*CalculateRelativeValue(resultValue, 
								primeMeasure, Sport.Core.Data.ResultMeasure.Centimeters, value);
						case Sport.Core.Data.ResultMeasure.Kilometers:
							return 1000*CalculateRelativeValue(resultValue, 
								primeMeasure, Sport.Core.Data.ResultMeasure.Meters, value);
						default:
							return value;
					}
					case Sport.Core.Data.ResultMeasure.Centimeters:
					switch (relativeMeasure)
					{
						case Sport.Core.Data.ResultMeasure.Meters:
							return 100*value;
						case Sport.Core.Data.ResultMeasure.Kilometers:
							return 1000*CalculateRelativeValue(resultValue, 
								primeMeasure, Sport.Core.Data.ResultMeasure.Meters, value);
						default:
							return value;
					}
					case Sport.Core.Data.ResultMeasure.Meters:
					switch (relativeMeasure)
					{
						case Sport.Core.Data.ResultMeasure.Kilometers:
							return 1000*value;
						default:
							return value;
					}
				} //end switch prime measure...
			}
			
			return value;
		} //end function CalculateRelativeValue
	}
}
