using System;
using System.Collections;
using System.Collections.Specialized;
using Sport.Rulesets.Rules;
using Sport.Rulesets;
using Sport.Common;

namespace Sport.Rulesets
{
	/// <summary>
	/// represents format of result using something like ##:##.### for example.
	/// </summary>
	public class ResultFormat
	{
		public static readonly char[] Delimeters=new char[] {'.', ':', ' '};
		public static readonly char DigitCharacter='#';
		
		StringCollection _parts;
		StringCollection _delimeters;
		
		public ResultFormat()
		{
			_parts = new StringCollection();
			_delimeters = new StringCollection();			
		}

		public ResultFormat(string format)
		{
			BuildFormat(format);
		}

		/// <summary>
		/// build the current instance of ResultFormat from given string.
		/// </summary>
		public void BuildFormat(string strFormat)
		{
			if (strFormat == null)
				throw new Exception("ResultFormat: string can't be null.");
			
			Sport.Common.Tools.SplitWithDelimeters(strFormat, Delimeters, 
				ref _parts, ref _delimeters);
		}

		/// <summary>
		/// returns whether given string match with the result format or not
		/// </summary>
		public bool Match(string value)
		{
			if (value.Length == 0)
				return true;
			
			//compare using regular expressions...
			string strPattern=BuildResultPattern();
			
			//build regular expression:
			System.Text.RegularExpressions.Regex objRegExp=
				new System.Text.RegularExpressions.Regex(strPattern);
			
			//return whether match result equals to the value itself...
			return (objRegExp.Match(value).ToString() == value);
		}

		/// <summary>
		/// returns whether given string match with the result format or not
		/// </summary>
		public bool PartialMatch(string value)
		{
			//valid if empty:
			if (value.Length == 0)
				return true;
			
			//split by delimeters:
			StringCollection arrValueParts=new StringCollection();
			StringCollection arrValueDelimeters=new StringCollection();
			Sport.Common.Tools.SplitWithDelimeters(value, Delimeters, 
				ref arrValueParts, ref arrValueDelimeters);

			//invalid if more parts than format:
			if (arrValueParts.Count > _parts.Count)
				return false;

			//invalid if more delimeters than format:
			if (arrValueDelimeters.Count > _delimeters.Count)
				return false;
			
			//compare parts:
			for (int partIndex=0; partIndex<arrValueParts.Count; partIndex++)
			{
				string curValuePart=arrValueParts[partIndex];
				string curFormatPart=_parts[partIndex];

				//invalid if more characters:
				if (curValuePart.Length > curFormatPart.Length)
					return false;

				//compare characters:
				for (int charIndex=0; charIndex<curValuePart.Length; charIndex++)
				{
					char curValueChar=curValuePart[charIndex];
					char curFormatChar=curFormatPart[charIndex];

					//check if both are digits:
					if ((Sport.Common.Tools.IsDigit(curValueChar))&&
						(curFormatChar == DigitCharacter))
						continue;
					
					//direct match:
					if (curValueChar != curFormatChar)
						return false;
				} //end loop over current part characters
			} //end loop over value parts

			//compare delimeters:
			for (int delimeterIndex=0; delimeterIndex<arrValueDelimeters.Count; 
				delimeterIndex++)
			{
				//invalid if not matching delimeter...
				if (arrValueDelimeters[delimeterIndex] != _delimeters[delimeterIndex])
					return false;
			}
			
			//partial match found...
			return true;
		}

		/// <summary>
		/// returns value filled with zeros.
		/// </summary>
		public string FillZeros(string value)
		{
			//must be valid:
			if (!PartialMatch(value))
				return value;
			
			//must be in correct format:
			if ((_parts.Count-1) != _delimeters.Count)
				return value;
			
			//split by delimeters:
			StringCollection arrValueParts=new StringCollection();
			StringCollection arrValueDelimeters=new StringCollection();
			Sport.Common.Tools.SplitWithDelimeters(value, Delimeters, 
				ref arrValueParts, ref arrValueDelimeters);
			
			//iterate over parts:
			string result="";
			int partIndex=0;
			while (partIndex < arrValueParts.Count)
			{
				string curValuePart=arrValueParts[partIndex];
				string curFormatPart=_parts[partIndex];

				if (partIndex == (_parts.Count-1))
				{
					//add part value:
					result += curValuePart;
					
					//fill current part:
					for (int charIndex=curValuePart.Length; 
						charIndex<curFormatPart.Length; charIndex++)
					{
						char curFormatChar=curFormatPart[charIndex];
						if (curFormatChar == DigitCharacter)
							result += "0";
						else
							result += curFormatChar.ToString();
					}
				}
				else
				{
					//fill current part:
					for (int charIndex=curValuePart.Length; 
						charIndex<curFormatPart.Length; charIndex++)
					{
						char curFormatChar=curFormatPart[charIndex];
						if (curFormatChar == DigitCharacter)
							result += "0";
						else
							result += curFormatChar.ToString();
					}
				
					//add part value:
					result += curValuePart;
				}
				
				//add the delimeter:
				if (partIndex < _delimeters.Count)
					result += _delimeters[partIndex].ToString();

				partIndex++;
			} //end loop over value parts

			//add rest of parts:
			while (partIndex < _parts.Count)
			{
				string curFormatPart=_parts[partIndex];
				
				//fill current part:
				for (int charIndex=0;  charIndex<curFormatPart.Length; charIndex++)
				{
					char curFormatChar=curFormatPart[charIndex];
					if (curFormatChar == DigitCharacter)
						result += "0";
					else
						result += curFormatChar.ToString();
				}
				
				//add the delimeter:
				if (partIndex < _delimeters.Count)
					result += _delimeters[partIndex].ToString();

				partIndex++;
			} //end loop over value parts
			
			return result;
		}

		/// <summary>
		/// convert given value to database result value
		/// </summary>
		public string ToResultValue(string value)
		{
			//simply replace any delimeter with proper seperator
			string result=value;
			for (int i=0; i<_delimeters.Count; i++)
			{
				result = result.Replace(_delimeters[i], Sport.Core.Data.ResultSeperator.ToString());
			}
			return result;
		}
		
		/// <summary>
		/// convert given database value to valid format, filling zeros.
		/// </summary>
		public string ValueToResult(string value)
		{
			//split by seperator:
			string[] arrValueParts=value.Split(new char[] {Sport.Core.Data.ResultSeperator});
			
			string result="";
			for (int i=0; i<arrValueParts.Length; i++)
			{
				//add value:
				result += arrValueParts[i];
				
				//abort if more than parts...
				if (i >= _delimeters.Count)
					break;
				
				//add the delimeter:
				result += _delimeters[i];
			}
			return FillZeros(result);
		}

		/// <summary>
		/// calculate the value of given result string.
		/// </summary>
		public int ResultValue(string value, Rules.ResultType resultType)
		{
			int result=0;
			string[] arrParts=value.Split(Delimeters);
			for (int i=0; i<arrParts.Length; i++)
			{
				string strPart=arrParts[i];
				result += CalculatePartValue(strPart, i, resultType);
			}
			return result;
		}

		/// <summary>
		/// calculate the value of given result string.
		/// </summary>
		public int ResultValue(string value)
		{
			return ResultValue(value, null);
		}

		private int CalculatePartValue(int partValue, int nestingLevel, 
			Rules.ResultType resultType, Sport.Core.Data.ResultValue resultValue)
		{
			if (resultType == null)
				return partValue;
			
			if (nestingLevel < 0)
				return partValue;
			
			//array of existing measures in the type:
			ArrayList arrMeasures=new ArrayList();
			
			if (resultValue == Sport.Core.Data.ResultValue.Time)
			{
				//build measures list:
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Days) != 0)
					arrMeasures.Add(Sport.Core.Data.ResultMeasure.Days);
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Hours) != 0)
					arrMeasures.Add(Sport.Core.Data.ResultMeasure.Hours);
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Minutes) != 0)
					arrMeasures.Add(Sport.Core.Data.ResultMeasure.Minutes);
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Seconds) != 0)
					arrMeasures.Add(Sport.Core.Data.ResultMeasure.Seconds);
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Miliseconds) != 0)
					arrMeasures.Add(Sport.Core.Data.ResultMeasure.Miliseconds);
			}

			if (resultValue == Sport.Core.Data.ResultValue.Distance)
			{
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Kilometers) != 0)
					arrMeasures.Add(Sport.Core.Data.ResultMeasure.Kilometers);
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Meters) != 0)
					arrMeasures.Add(Sport.Core.Data.ResultMeasure.Meters);
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Centimeters) != 0)
					arrMeasures.Add(Sport.Core.Data.ResultMeasure.Centimeters);
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Milimeters) != 0)
					arrMeasures.Add(Sport.Core.Data.ResultMeasure.Milimeters);
			}
			
			//abort if invalid index:
			if (nestingLevel >= arrMeasures.Count)
				return 0;
			
			Sport.Core.Data.ResultMeasure primeValue=(Sport.Core.Data.ResultMeasure) arrMeasures[arrMeasures.Count-1];
			Sport.Core.Data.ResultMeasure relativeValue=(Sport.Core.Data.ResultMeasure) arrMeasures[nestingLevel];
			return Tools.CalculateRelativeValue(resultValue, primeValue, relativeValue, partValue);
		}

		private int CalculatePartValue(string strPart, int nestingLevel, 
			Rules.ResultType resultType)
		{
			int partValue=Sport.Common.Tools.CIntDef(strPart, -1);
			if (partValue < 0)
				return 0;
			
			if (resultType == null)
				return partValue;
			
			int result=0;
			
			switch (resultType.Value)
			{
				case Sport.Core.Data.ResultValue.Time:
				case Sport.Core.Data.ResultValue.Distance:
					result = CalculatePartValue(partValue, nestingLevel, resultType, 
						resultType.Value);
					break;
				case Sport.Core.Data.ResultValue.Points:
					result = 0;
					break;
			}
			
			return result;
		}
		
		public override bool Equals(object obj)
		{
			if (!(obj is ResultFormat))
				return false;
			if (obj == null)
				return false;
			ResultFormat format=(ResultFormat) obj;
			return ((format._parts.Equals(this._parts))&&
				(format._delimeters.Equals(this._delimeters)));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override string ToString()
		{
			string result="";
			for (int i=0; i<_parts.Count; i++)
			{
				result += _parts[i];
				if (i < _delimeters.Count)
					result += _delimeters[i];
			}
			return result;
		}
		
		/// <summary>
		/// build regular expression pattern out of the result format
		/// </summary>
		private string BuildResultPattern()
		{
			string strPattern="^"; //this character means the string must begin with the following
			string strMyValue=this.ToString(); //get string with ResultFormat value
			int lowerBound=1; //used to group the digits
			int upperBound=0; //used to group the digits
			System.Collections.ArrayList arrDelimeters=
				new System.Collections.ArrayList(Delimeters);
			
			//build the pattern. group the digits between the delimeters.
			//for example ##:##.### would become [0-9]{1,2}[:][0-9]{1,2}[.][0-9]{1,3}
			//because we have to allow something like 5:50.10 and 5:50.100
			for (int i=0; i<strMyValue.Length; i++)
			{
				char curChar=strMyValue[i];

				//current character is digit?
				if (curChar == DigitCharacter)
				{
					//raise group counter...
					upperBound++;
					continue;
				}

				//current character is delimeter?
				if (arrDelimeters.IndexOf(curChar) >= 0)
				{
					//need to add group of digits to the pattern if valid:
					if (upperBound >= lowerBound)
						strPattern += "[0-9]{"+lowerBound+","+upperBound+"}";

					//reset counter and flag:
					upperBound=0;
					//delimeterFound = false;
				} //end if delimeter
				
				//character is not digit, add to pattern as is:
				strPattern += "["+curChar.ToString()+"]";
			} //end loop over format characters
			
			//add last digits group as well:
			if (upperBound >= lowerBound)
				strPattern += "[0-9]{"+lowerBound+","+upperBound+"}";
			
			//this character means: the string must end as it is.
			strPattern += "$";
			return strPattern;
		}
	}

	/// <summary>
	/// Summary description for Result.
	/// </summary>
	public class Result
	{
		public Result()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
